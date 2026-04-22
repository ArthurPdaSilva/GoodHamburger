import {
	Card,
	CardContent,
	List,
	ListItem,
	ListItemButton,
	ListItemText,
	Typography,
} from "@mui/material";
import type { MenuItem, MenuItemTypeCode } from "../types";
import { typeLabelMap } from "../types";
import { formatCurrency } from "../utils/formatCurrency";
import { normalizeTypeCode } from "../utils/normalizeTypeCode";

type MenuItemsSectionProps = {
	menuItems: MenuItem[];
	selectedByType: Partial<Record<MenuItemTypeCode, MenuItem>>;
	onSelectItem: (item: MenuItem) => void;
};

export const MenuItemsSection = ({
	menuItems,
	selectedByType,
	onSelectItem,
}: MenuItemsSectionProps) => {
	return (
		<Card>
			<CardContent>
				<Typography variant="h6" sx={{ mb: 2, fontWeight: 600 }}>
					Itens do Cardapio
				</Typography>

				<List disablePadding>
					{menuItems.map((item) => {
						const type = normalizeTypeCode(item.type);
						const isSelected = selectedByType[type]?.id === item.id;

						return (
							<ListItem key={item.id} disablePadding divider>
								<ListItemButton
									onClick={() => onSelectItem(item)}
									selected={isSelected}
								>
									<ListItemText
										primary={`${item.name} - ${formatCurrency(item.price)}`}
										secondary={typeLabelMap[type]}
									/>
								</ListItemButton>
							</ListItem>
						);
					})}
				</List>
			</CardContent>
		</Card>
	);
};
