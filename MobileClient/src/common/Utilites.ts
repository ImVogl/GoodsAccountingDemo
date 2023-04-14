// Getting base url fron config.
export function getBaseUrl(): string{
    const config = require('../config.json');
    return config.use_tls ? config.server_url_ssl : config.server_url;
}

// Getting full path to TLS certificate.
export function getCertificatePath(): string{
    return "I:\\Visual Studio 2017\\DemostrationProjects\\GoodsAccountingDemo\\Certificate\\server.crt";
}

// Getting full path to TLS secret key.
export function getSecretKeyPath(): string{
    return "I:\\Visual Studio 2017\\DemostrationProjects\\GoodsAccountingDemo\\Certificate\\server.key";
}