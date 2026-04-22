import {
	Alert,
	Box,
	Button,
	Card,
	CardContent,
	CircularProgress,
	Grid,
	Stack,
	Typography,
} from "@mui/material";
import { useEffect, useMemo, useState } from "react";
import { Link as RouterLink, useParams } from "react-router-dom";
import { MenuItemsSection } from "../components/MenuItemsSection";
import { OrderSummarySection } from "../components/OrderSummarySection";
import { getMenuItems, getOrderById, updateOrder } from "../services/api";
import type {
	MenuItem,
	MenuItemTypeCode,
	OrderInput,
	OrderItemInput,
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

const mapItemToMenuItem = (item: OrderItemInput): MenuItem => ({
	id: item.menuItemId,
	name: item.name,
	price: item.price,
	type: item.type,
});

export const OrderDetailsPage = () => {
	const { id } = useParams<{ id: string }>();

	const [menuItems, setMenuItems] = useState<MenuItem[]>([]);
	const [selectedByType, setSelectedByType] = useState<
		Partial<Record<MenuItemTypeCode, MenuItem>>
	>({});

	const [loading, setLoading] = useState(true);
	const [savingOrder, setSavingOrder] = useState(false);
	const [error, setError] = useState<string | null>(null);
	const [successMessage, setSuccessMessage] = useState<string | null>(null);

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
		if (!id) {
			setError("ID do pedido invalido.");
			setLoading(false);
			return;
		}

		let mounted = true;

		Promise.all([getMenuItems(), getOrderById(id)])
			.then(([menuData, orderData]) => {
				if (!mounted) {
					return;
				}

				setMenuItems(menuData);

				const selectedMap: Partial<Record<MenuItemTypeCode, MenuItem>> = {};
				for (const orderItem of orderData.items) {
					const type = normalizeTypeCode(orderItem.type);
					const menuMatch = menuData.find(
						(menuItem) => menuItem.id === orderItem.menuItemId,
					);
					selectedMap[type] = menuMatch ?? mapItemToMenuItem(orderItem);
				}

				setSelectedByType(selectedMap);
			})
			.catch((err: unknown) => {
				if (mounted) {
					setError(
						err instanceof Error
							? err.message
							: "Erro ao carregar pedido para edicao.",
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
	}, [id]);

	const handleSelectItem = (item: MenuItem) => {
		const type = normalizeTypeCode(item.type);
		setSelectedByType((current) => ({
			...current,
			[type]: item,
		}));
	};

	const clearSelection = () => {
		setSelectedByType({});
	};

	const handleUpdateOrder = async () => {
		if (!id) {
			setError("ID do pedido invalido.");
			return;
		}

		if (selectedItems.length === 0) {
			setError("Selecione ao menos um item para atualizar o pedido.");
			return;
		}

		setSavingOrder(true);
		setError(null);
		setSuccessMessage(null);

		const payload: OrderInput = {
			id,
			subTotal,
			total: estimatedTotal,
			items: selectedItems.map((item) => ({
				id: crypto.randomUUID(),
				menuItemId: item.id,
				type: normalizeTypeCode(item.type),
				name: item.name,
				price: item.price,
			})),
		};

		try {
			await updateOrder(id, payload);
			setSuccessMessage("Pedido atualizado com sucesso.");
		} catch (err: unknown) {
			setError(
				err instanceof Error ? err.message : "Falha ao atualizar pedido.",
			);
		} finally {
			setSavingOrder(false);
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
					Editar Pedido
				</Typography>
				<Typography color="text.secondary">
					Ajuste os itens e salve as alteracoes do pedido {id?.slice(0, 8)}.
				</Typography>
			</Box>

			{error && <Alert severity="error">{error}</Alert>}
			{successMessage && <Alert severity="success">{successMessage}</Alert>}

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
						onSubmitOrder={handleUpdateOrder}
						savingOrder={savingOrder}
						selectedItems={selectedItems}
						submitButtonLabel="Atualizar Pedido"
						subTotal={subTotal}
					/>
				</Grid>
			</Grid>

			<Card>
				<CardContent>
					<Stack direction="row" spacing={1}>
						<Button component={RouterLink} to="/pedidos" variant="outlined">
							Voltar para pedidos
						</Button>
					</Stack>
				</CardContent>
			</Card>
		</Stack>
	);
};
