import {
	Button,
	Card,
	CardContent,
	Stack,
	Table,
	TableBody,
	TableCell,
	TableHead,
	TableRow,
	Typography,
} from "@mui/material";
import { Link as RouterLink } from "react-router-dom";
import type { OrderList } from "../types";
import { formatCurrency } from "../utils/formatCurrency";

type RegisteredOrdersSectionProps = {
	orders: OrderList[];
	deletingOrderId: string | null;
	onDeleteOrder: (id: string) => void;
};

export const RegisteredOrdersSection = ({
	orders,
	deletingOrderId,
	onDeleteOrder,
}: RegisteredOrdersSectionProps) => {
	return (
		<Card>
			<CardContent>
				<Typography variant="h6" sx={{ mb: 2, fontWeight: 600 }}>
					Pedidos Registrados
				</Typography>

				{orders.length === 0 ? (
					<Typography color="text.secondary">
						Nenhum pedido cadastrado.
					</Typography>
				) : (
					<Table size="small">
						<TableHead>
							<TableRow>
								<TableCell>ID</TableCell>
								<TableCell>Itens</TableCell>
								<TableCell align="right">Subtotal</TableCell>
								<TableCell align="right">Total</TableCell>
								<TableCell align="right">Ações</TableCell>
							</TableRow>
						</TableHead>
						<TableBody>
							{orders.map((order) => (
								<TableRow key={order.id}>
									<TableCell>{order.id.slice(0, 8)}</TableCell>
									<TableCell>{order.itemsCount}</TableCell>
									<TableCell align="right">
										{formatCurrency(order.subTotal)}
									</TableCell>
									<TableCell align="right">
										{formatCurrency(order.total)}
									</TableCell>
									<TableCell align="right">
										<Stack
											direction="row"
											spacing={1}
											sx={{ justifyContent: "flex-end" }}
										>
											<Button
												component={RouterLink}
												to={`/pedidos/${order.id}`}
												size="small"
												variant="contained"
											>
												Editar
											</Button>
											<Button
												color="error"
												disabled={deletingOrderId === order.id}
												onClick={() => onDeleteOrder(order.id)}
												size="small"
												variant="outlined"
											>
												{deletingOrderId === order.id
													? "Removendo..."
													: "Remover"}
											</Button>
										</Stack>
									</TableCell>
								</TableRow>
							))}
						</TableBody>
					</Table>
				)}
			</CardContent>
		</Card>
	);
};
