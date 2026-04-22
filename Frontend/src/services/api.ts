import type { MenuItem, OrderInput, OrderList } from "../types";

const API_BASE_URL = import.meta.env.VITE_API_URL ?? "http://localhost:5257";

const request = async <T>(path: string, init?: RequestInit): Promise<T> => {
	const response = await fetch(`${API_BASE_URL}${path}`, {
		headers: {
			"Content-Type": "application/json",
			...(init?.headers ?? {}),
		},
		...init,
	});

	if (!response.ok) {
		const message = await response.text();
		throw new Error(message || "Falha na comunicacao com a API.");
	}

	if (response.status === 204) {
		return undefined as T;
	}

	const hasBody = response.headers.get("content-length") !== "0";
	return (hasBody ? response.json() : undefined) as Promise<T>;
};

export const getMenuItems = (): Promise<MenuItem[]> => {
	return request<MenuItem[]>("/api/menuitems");
};

export const getOrders = (): Promise<OrderList[]> => {
	return request<OrderList[]>("/api/orders");
};

export const getOrderById = (id: string): Promise<OrderInput> => {
	return request<OrderInput>(`/api/orders/${id}`);
};

export const createOrder = (payload: OrderInput): Promise<void> => {
	return request<void>("/api/orders", {
		method: "POST",
		body: JSON.stringify(payload),
	});
};

export const deleteOrder = (id: string): Promise<void> => {
	return request<void>(`/api/orders/${id}`, {
		method: "DELETE",
	});
};

export const updateOrder = (id: string, payload: OrderInput): Promise<void> => {
	return request<void>(`/api/orders/${id}`, {
		method: "PUT",
		body: JSON.stringify(payload),
	});
};
