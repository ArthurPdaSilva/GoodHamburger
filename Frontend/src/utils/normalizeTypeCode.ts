import type { MenuItem, MenuItemTypeCode } from "../types";

export const normalizeTypeCode = (type: MenuItem["type"]): MenuItemTypeCode => {
	if (typeof type === "number") {
		return type as MenuItemTypeCode;
	}

	const lower = type.toLowerCase();
	if (lower === "main") return 1;
	if (lower === "side") return 2;
	return 3;
};
