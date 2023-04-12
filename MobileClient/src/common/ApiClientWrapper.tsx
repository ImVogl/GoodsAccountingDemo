
class ApiClientWrapper{
    public logUser(login: string, password: string): Promise<boolean>{
        return Promise.resolve(false);
    }
}

export default ApiClientWrapper;