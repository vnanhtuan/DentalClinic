export async function loadComponent(url) {
    const response = await fetch(url);
    const html = await response.text();
    return {
        template: html,
        data() {
            return {}
        }
    }
}