import { Navigate, Route, Routes } from "react-router-dom";
import { MenuPage, OrderDetailsPage, OrdersPage } from "../pages";

export const RoutesContainer = () => {
	return (
		<Routes>
			<Route path="/" element={<Navigate to="/cardapio" replace />} />
			<Route path="/cardapio" element={<MenuPage />} />
			<Route path="/pedidos" element={<OrdersPage />} />
			<Route path="/pedidos/:id" element={<OrderDetailsPage />} />
		</Routes>
	);
};
