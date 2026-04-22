import {
	Alert,
	Box,
	CircularProgress,
	Grid,
	Snackbar,
	Stack,
	Typography,
} from "@mui/material";
import { useEffect, useMemo, useState } from "react";
import { MenuItemsSection } from "../components/MenuItemsSection";
import { OrderSummarySection } from "../components/OrderSummarySection";
import { RegisteredOrdersSection } from "../components/RegisteredOrdersSection";
import {
	createOrder,
	deleteOrder,
	getMenuItems,
	getOrders,
} from "../services/api";
import {
	type MenuItem,
	type MenuItemTypeCode,
	type OrderInput,
	type OrderList,
	typeLabelMap,
} from "../types";
import { normalizeTypeCode } from "../utils/normalizeTypeCode";

const calculateTotalWithDiscount = (selectedItems: MenuItem[]): number => {
	const selectedTypes = new Set(
		selectedItems.map((item) => normalizeTypeCode(item.type)),
	);
	const subTotal = selectedItems.reduce((sum, item) => sum + item.price, 0);

	const hasMain = selectedTypes.has(1);
	const hasSide = selectedTypes.has(2);
	const hasDrink = selectedTypes.has(3);

	if (hasMain && hasSide && hasDrink) {
		return subTotal * 0.8;
	}

	if (hasMain && hasDrink) {
		return subTotal * 0.85;
	}

	if (hasMain && hasSide) {
		return subTotal * 0.9;
	}

	return subTotal;
};

export const OrdersPage = () => {
	const [menuItems, setMenuItems] = useState<MenuItem[]>([]);
	const [orders, setOrders] = useState<OrderList[]>([]);
	const [selectedByType, setSelectedByType] = useState<
		Partial<Record<MenuItemTypeCode, MenuItem>>
	>({});

	const [loading, setLoading] = useState(true);
	const [savingOrder, setSavingOrder] = useState(false);
	const [deletingOrderId, setDeletingOrderId] = useState<string | null>(null);

	const [error, setError] = useState<string | null>(null);
	const [snackbarMessage, setSnackbarMessage] = useState<string | null>(null);

	const selectedItems = useMemo(
		() => Object.values(selectedByType),
		[selectedByType],
	);
	const subTotal = useMemo(
		() => selectedItems.reduce((sum, item) => sum + item.price, 0),
		[selectedItems],
	);
	const estimatedTotal = useMemo(
		() => calculateTotalWithDiscount(selectedItems),
		[selectedItems],
	);

	useEffect(() => {
		let mounted = true;

		Promise.all([getMenuItems(), getOrders()])
			.then(([menuData, orderData]) => {
				if (mounted) {
					setMenuItems(menuData);
					setOrders(orderData);
				}
			})
			.catch((err: unknown) => {
				if (mounted) {
					setError(
						err instanceof Error
							? err.message
							: "Erro ao carregar dados de pedidos.",
					);
				}
			})
			.finally(() => {
				if (mounted) {
					setLoading(false);
				}
			});

		return () => {
			mounted = false;
		};
	}, []);

	const handleSelectItem = (item: MenuItem) => {
		const type = normalizeTypeCode(item.type);

		setSelectedByType((current) => ({
			...current,
			[type]: item,
		}));
		setSnackbarMessage(`${item.name} selecionado em ${typeLabelMap[type]}.`);
	};

	const clearSelection = () => {
		setSelectedByType({});
	};

	const submitOrder = async () => {
		if (selectedItems.length === 0) {
			setSnackbarMessage("Selecione ao menos um item para registrar o pedido.");
			return;
		}

		setSavingOrder(true);
		setError(null);

		const payload: OrderInput = {
			id: crypto.randomUUID(),
			subTotal: 0,
			total: 0,
			items: selectedItems.map((item) => ({
				id: crypto.randomUUID(),
				menuItemId: item.id,
				type: normalizeTypeCode(item.type),
				name: item.name,
				price: item.price,
			})),
		};

		try {
			await createOrder(payload);
			const updatedOrders = await getOrders();
			setOrders(updatedOrders);
			clearSelection();
			setSnackbarMessage("Pedido registrado com sucesso.");
		} catch (err: unknown) {
			setError(
				err instanceof Error ? err.message : "Falha ao registrar pedido.",
			);
		} finally {
			setSavingOrder(false);
		}
	};

	const handleDeleteOrder = async (id: string) => {
		setDeletingOrderId(id);
		setError(null);

		try {
			await deleteOrder(id);
			const updatedOrders = await getOrders();
			setOrders(updatedOrders);
			setSnackbarMessage("Pedido removido com sucesso.");
		} catch (err: unknown) {
			setError(err instanceof Error ? err.message : "Falha ao remover pedido.");
		} finally {
			setDeletingOrderId(null);
		}
	};

	if (loading) {
		return (
			<Box sx={{ display: "flex", justifyContent: "center", py: 12 }}>
				<CircularProgress />
			</Box>
		);
	}

	return (
		<Stack spacing={3}>
			<Box>
				<Typography variant="h4" sx={{ fontWeight: 700 }}>
					Registro de Pedidos
				</Typography>
				<Typography color="text.secondary">
					Selecione no maximo um item de cada tipo e finalize para gerar um novo
					pedido.
				</Typography>
			</Box>

			{error && <Alert severity="error">{error}</Alert>}

			<Grid container spacing={3}>
				<Grid size={{ xs: 12, md: 7 }}>
					<MenuItemsSection
						menuItems={menuItems}
						onSelectItem={handleSelectItem}
						selectedByType={selectedByType}
					/>
				</Grid>

				<Grid size={{ xs: 12, md: 5 }}>
					<OrderSummarySection
						estimatedTotal={estimatedTotal}
						onClearSelection={clearSelection}
						onSubmitOrder={submitOrder}
						savingOrder={savingOrder}
						selectedItems={selectedItems}
						subTotal={subTotal}
					/>
				</Grid>
			</Grid>

			<RegisteredOrdersSection
				deletingOrderId={deletingOrderId}
				onDeleteOrder={handleDeleteOrder}
				orders={orders}
			/>

			<Snackbar
				autoHideDuration={2500}
				message={snackbarMessage ?? ""}
				onClose={() => setSnackbarMessage(null)}
				open={Boolean(snackbarMessage)}
			/>
		</Stack>
	);
};
