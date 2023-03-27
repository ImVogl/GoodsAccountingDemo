export class BadRequest extends Error{
    constructor(msg: string) {
        super(msg);

        // Set the prototype explicitly.
        Object.setPrototypeOf(this, BadRequest.prototype);
    }
}