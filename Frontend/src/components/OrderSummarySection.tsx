import {
	Box,
	Button,
	Card,
	CardContent,
	Divider,
	Stack,
	Typography,
} from "@mui/material";
import type { MenuItem } from "../types";
import { typeLabelMap } from "../types";
import { formatCurrency } from "../utils/formatCurrency";
import { normalizeTypeCode } from "../utils/normalizeTypeCode";

type OrderSummarySectionProps = {
	selectedItems: MenuItem[];
	subTotal: number;
	estimatedTotal: number;
	savingOrder: boolean;
	onClearSelection: () => void;
	onSubmitOrder: () => void;
	submitButtonLabel?: string;
};

export const OrderSummarySection = ({
	selectedItems,
	subTotal,
	estimatedTotal,
	savingOrder,
	onClearSelection,
	onSubmitOrder,
	submitButtonLabel,
}: OrderSummarySectionProps) => {
	const resolvedSubmitLabel = submitButtonLabel ?? "Registrar Pedido";

	return (
		<Card>
			<CardContent>
				<Typography variant="h6" sx={{ mb: 2, fontWeight: 600 }}>
					Resumo do Pedido
				</Typography>

				<Stack spacing={1.5} sx={{ mb: 2 }}>
					{selectedItems.length === 0 && (
						<Typography color="text.secondary">
							Nenhum item selecionado.
						</Typography>
					)}

					{selectedItems.map((item) => {
						const type = normalizeTypeCode(item.type);
						return (
							<Box key={type}>
								<Typography sx={{ fontWeight: 600 }}>
									{typeLabelMap[type]}
								</Typography>
								<Typography color="text.secondary">
									{item.name} - {formatCurrency(item.price)}
								</Typography>
							</Box>
						);
					})}
				</Stack>

				<Divider sx={{ my: 2 }} />

				<Stack spacing={0.5} sx={{ mb: 2.5 }}>
					<Typography>Subtotal: {formatCurrency(subTotal)}</Typography>
					<Typography sx={{ fontWeight: 700 }}>
						Total com desconto: {formatCurrency(estimatedTotal)}
					</Typography>
				</Stack>

				<Stack direction="row" spacing={1}>
					<Button
						variant="outlined"
						onClick={onClearSelection}
						disabled={savingOrder}
					>
						Limpar
					</Button>
					<Button
						variant="contained"
						color="primary"
						onClick={onSubmitOrder}
						disabled={savingOrder}
					>
						{savingOrder ? "Salvando..." : resolvedSubmitLabel}
					</Button>
				</Stack>
			</CardContent>
		</Card>
	);
};
