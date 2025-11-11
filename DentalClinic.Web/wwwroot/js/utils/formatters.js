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

export function urlEncodeParams(params) {
    const qp = new URLSearchParams();
    if (params) {
        Object.keys(params).forEach(key => {
            const value = params[key];
            if (value === undefined || value === null) return;
            if (Array.isArray(value)) {
                value.forEach(v => qp.append(key, v));
            } else {
                qp.append(key, value);
            }
        });
    }
    return qp.toString();
}