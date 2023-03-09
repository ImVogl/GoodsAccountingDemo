
// Getting base url fron config.
export function getBaseUrl(): string{
    const config = require('../../config.json');
    return config.use_tls ? config.server_url_ssl : config.server_url;
}

// Run timer.
export const sleep_ms = (ms:number) => new Promise(res => setTimeout(res, ms));
