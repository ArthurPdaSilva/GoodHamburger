import { createTheme } from "@mui/material";

export const theme = createTheme({
	palette: {
		mode: "light",
		primary: {
			main: "#b23a48",
		},
		secondary: {
			main: "#fcbf49",
		},
		background: {
			default: "#f8f4f1",
			paper: "#ffffff",
		},
	},
	shape: {
		borderRadius: 14,
	},
	typography: {
		fontFamily: '"Segoe UI", "Trebuchet MS", Tahoma, sans-serif',
	},
});
