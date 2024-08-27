const colors = require('tailwindcss/colors');
const svelte_ux = require('svelte-ux/plugins/tailwind.cjs');

/** @type {import('tailwindcss').Config}*/
const config = {
	content: ['./src/**/*.{html,svelte}', './node_modules/svelte-ux/**/*.{svelte,js}'],
	theme: {
		extend: {}
	},
	variants: {
		extend: {}
	},
	plugins: [svelte_ux],
	ux: {
		themes: {
			"light": {
				"color-scheme": "light",
				"primary": "hsl(217.2193 91.2195% 59.8039%)",
				"secondary": "hsl(198.6301 88.664% 48.4314%)",
				"accent": "hsl(238.7324 83.5294% 66.6667%)",
				"surface-100": "hsl(210 20% 98.0392%)",
				"surface-200": "hsl(220 14.2857% 95.8824%)",
				"surface-300": "hsl(220 13.0435% 90.9804%)"
			},
			"dark": {
				"color-scheme": "dark",
				"primary": "hsl(217.2193 91.2195% 59.8039%)",
				"secondary": "hsl(198.6301 88.664% 48.4314%)",
				"accent": "hsl(238.7324 83.5294% 66.6667%)",
				"surface-100": "hsl(216.9231 19.1176% 26.6667%)",
				"surface-200": "hsl(215 27.907% 16.8627%)",
				"surface-300": "hsl(220.9091 39.2857% 10.9804%)"
			}
		}
	}
};

module.exports = config;
