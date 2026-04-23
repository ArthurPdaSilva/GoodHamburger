import {
	Box,
	Container
} from "@mui/material";
import { Header } from "./components/Header";
import { RoutesContainer } from "./routes";

export const App = () => {
	

	return (
		<Box sx={{ minHeight: "100vh" }}>
			<Header />
			<Container maxWidth="lg" sx={{ py: 4 }}>
				<RoutesContainer />
			</Container>
		</Box>
	);
};
