import { containsSpecSymbol, lengthEnought, containsLetter, containsDigit } from './validation'

it('Checking expression', () => {
    expect(lengthEnought("Az!2.sA")).toEqual(false);
    expect(containsSpecSymbol("Az2sssA")).toEqual(false);
    expect(containsLetter("!2.22222234")).toEqual(false);
    expect(containsDigit("Az!R.sssA")).toEqual(false);
});