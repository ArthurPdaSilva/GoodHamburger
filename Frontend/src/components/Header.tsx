import { AppBar, Box, Button, Toolbar, Typography } from "@mui/material";
import { Link, useLocation } from "react-router";

export const Header = () => {
	const location = useLocation();
	const isOrdersRoute = location.pathname.startsWith("/pedidos");

	return (
		<AppBar position="static" elevation={0}>
			<Toolbar
				sx={{ display: "flex", justifyContent: "space-between", gap: 2 }}
			>
				<Button component={Link} to="/">
					<img src="/logo.png" alt="Logo" style={{ width: 40, height: 40 }} />

					<Typography
						variant="h6"
						component="h1"
						sx={{ fontWeight: 700, color: "white", ml: 1 }}
					>
						Good Hamburger
					</Typography>
				</Button>

				<Box sx={{ display: "flex", gap: 1 }}>
					<Button
						color={location.pathname === "/cardapio" ? "secondary" : "inherit"}
						component={Link}
						to="/cardapio"
						variant={location.pathname === "/cardapio" ? "contained" : "text"}
					>
						Cardapio
					</Button>
					<Button
						color={isOrdersRoute ? "secondary" : "inherit"}
						component={Link}
						to="/pedidos"
						variant={isOrdersRoute ? "contained" : "text"}
					>
						Pedidos
					</Button>
				</Box>
			</Toolbar>
		</AppBar>
	);
};
