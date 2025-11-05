/**
 * Format VND.
 */
export function formatCurrency(value) {
    if (typeof value !== 'number') {
        value = 0;
    }
    return new Intl.NumberFormat('vi-VN', {
        style: 'currency',
        currency: 'VND'
    }).format(value);
}