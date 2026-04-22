import {
	Alert,
	Box,
	Card,
	CardContent,
	Chip,
	CircularProgress,
	Grid,
	Stack,
	Typography,
} from "@mui/material";
import { useEffect, useMemo, useState } from "react";
import { getMenuItems } from "../services/api";
import { type MenuItem, type MenuItemTypeCode, typeLabelMap } from "../types";
import { formatCurrency } from "../utils/formatCurrency";
import { normalizeTypeCode } from "../utils/normalizeTypeCode";

export const MenuPage = () => {
	const [menuItems, setMenuItems] = useState<MenuItem[]>([]);
	const [loading, setLoading] = useState(true);
	const [error, setError] = useState<string | null>(null);

	useEffect(() => {
		let mounted = true;

		getMenuItems()
			.then((data) => {
				if (mounted) {
					setMenuItems(data);
				}
			})
			.catch((err: unknown) => {
				if (mounted) {
					setError(
						err instanceof Error ? err.message : "Erro ao carregar cardapio.",
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

	const groupedItems = useMemo(() => {
		return menuItems.reduce<Record<MenuItemTypeCode, MenuItem[]>>(
			(acc, item) => {
				const type = normalizeTypeCode(item.type);
				acc[type].push(item);
				return acc;
			},
			{
				1: [],
				2: [],
				3: [],
			},
		);
	}, [menuItems]);

	if (loading) {
		return (
			<Box sx={{ display: "flex", justifyContent: "center", py: 12 }}>
				<CircularProgress />
			</Box>
		);
	}

	if (error) {
		return <Alert severity="error">{error}</Alert>;
	}

	return (
		<Stack spacing={4}>
			<Box>
				<Typography variant="h4" sx={{ fontWeight: 700 }}>
					Cardapio Good Hamburger
				</Typography>
				<Typography color="text.secondary">
					Escolha entre hamburguer, acompanhamento e bebida para montar pedidos.
				</Typography>
			</Box>

			{(Object.keys(groupedItems) as unknown as MenuItemTypeCode[]).map(
				(typeKey) => {
					const items = groupedItems[typeKey];
					return (
						<Box key={typeKey}>
							<Typography variant="h5" sx={{ fontWeight: 600, mb: 2 }}>
								{typeLabelMap[typeKey]}
							</Typography>

							<Grid container spacing={2}>
								{items.map((item) => (
									<Grid size={{ xs: 12, sm: 6, md: 4 }} key={item.id}>
										<Card sx={{ height: "100%" }}>
											<CardContent>
												<Box
													sx={{
														display: "flex",
														justifyContent: "space-between",
														alignItems: "center",
														gap: 1,
													}}
												>
													<Typography variant="h6" sx={{ fontWeight: 600 }}>
														{item.name}
													</Typography>
													<Chip
														label={typeLabelMap[normalizeTypeCode(item.type)]}
														color="secondary"
														size="small"
													/>
												</Box>
												<Typography variant="body1" sx={{ mt: 1.5 }}>
													{formatCurrency(item.price)}
												</Typography>
											</CardContent>
										</Card>
									</Grid>
								))}
							</Grid>
						</Box>
					);
				},
			)}
		</Stack>
	);
};
