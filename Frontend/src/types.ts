export type MenuItemTypeCode = 1 | 2 | 3;

export type MenuItem = {
	id: string;
	name: string;
	price: number;
	type: MenuItemTypeCode | string;
};

export type OrderItemInput = {
	id: string;
	menuItemId: string;
	type: MenuItemTypeCode;
	name: string;
	price: number;
};

export type OrderInput = {
	id: string;
	subTotal: number;
	total: number;
	items: OrderItemInput[];
};

export type OrderListItem = {
	id: string;
	menuItemId: string;
	type: MenuItemTypeCode | string;
	name: string;
	price: number;
};

export type OrderList = {
	id: string;
	subTotal: number;
	total: number;
	itemsCount: number;
	items: OrderListItem[];
};

export const typeLabelMap: Record<MenuItemTypeCode, string> = {
	1: "Hamburguer",
	2: "Acompanhamento",
	3: "Bebida",
};
