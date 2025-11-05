export const vuetify = Vuetify.createVuetify({
    theme: {
        defaultTheme: "light",
        themes: {
            light: {
                colors: {
                    primary: "#0d9488",
                    secondary: "#ffb300",
                },
            },
        },
    },
    defaults: { global: { ripple: true } },
    icons: { defaultSet: "mdi" },
});