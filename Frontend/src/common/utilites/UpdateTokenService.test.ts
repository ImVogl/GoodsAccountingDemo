import { TokenUpdater } from './UpdateTokenService'
import { UpdateClient } from './SwaggerClient';
import { expect, jest, test } from '@jest/globals'

describe('Token updater main loop test.', () => {
    const token_key = "token";
    const token = "2e3dca51-2b7d-41c2-83ef-f9254e1e4682";
    const newToken = "d46e5b41-869b-4492-9738-7e69289262c9";
    const client = new UpdateClient();
    let resultPromise = new Promise<{[key: string]: string}>((resolve, reject) => resolve({"token": newToken}))
    let spy = jest.spyOn(client, 'token').mockImplementation(() => resultPromise);
    test('Update token cycle test', async () => {
        const tokenUpdater = new TokenUpdater(null, client);
        tokenUpdater.prepare(token);
        expect(window.localStorage.getItem(token_key)).toEqual(token);
        await tokenUpdater.startTokenUpdater();
        expect(window.localStorage.getItem(token_key)).toEqual(newToken);
        tokenUpdater.reset();
        expect(window.localStorage.getItem(token_key)).toBeNull();
        spy.mockRestore();
    });
});