import {
	AppBar,
	Box,
	Button,
	Container,
	Toolbar,
	Typography,
} from "@mui/material";
import { Link as RouterLink, useLocation } from "react-router-dom";
import { RoutesContainer } from "./routes";

export const App = () => {
	const location = useLocation();
	const isOrdersRoute = location.pathname.startsWith("/pedidos");

	return (
		<Box sx={{ minHeight: "100vh" }}>
			<AppBar position="static" elevation={0}>
				<Toolbar
					sx={{ display: "flex", justifyContent: "space-between", gap: 2 }}
				>
					<Typography variant="h6" component="h1" sx={{ fontWeight: 700 }}>
						Good Hamburger
					</Typography>

					<Box sx={{ display: "flex", gap: 1 }}>
						<Button
							color={
								location.pathname === "/cardapio" ? "secondary" : "inherit"
							}
							component={RouterLink}
							to="/cardapio"
							variant={location.pathname === "/cardapio" ? "contained" : "text"}
						>
							Cardapio
						</Button>
						<Button
							color={isOrdersRoute ? "secondary" : "inherit"}
							component={RouterLink}
							to="/pedidos"
							variant={isOrdersRoute ? "contained" : "text"}
						>
							Pedidos
						</Button>
					</Box>
				</Toolbar>
			</AppBar>

			<Container maxWidth="lg" sx={{ py: 4 }}>
				<RoutesContainer />
			</Container>
		</Box>
	);
};
