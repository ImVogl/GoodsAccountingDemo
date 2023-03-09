import { containsSpecSymbol, lengthEnought, containsLetter, containsDigit } from './validation'

it('Checking expression - length', () => {
    expect(lengthEnought("Az!2.sA")).toEqual(false);
    expect(lengthEnought("Az!2.sAa")).toEqual(true);
});

it('Checking expression - digits', () => {
    expect(containsDigit("Az!R.sssA")).toEqual(false);
    expect(containsDigit("Az!R2.sssA")).toEqual(true);
});

it('Checking expression - letters', () => {
    expect(containsLetter("!2.22222234")).toEqual(false);
    expect(containsLetter("a")).toEqual(false);
    expect(containsLetter("A")).toEqual(false);
    expect(containsLetter("Aa")).toEqual(true);
});

it('Checking expression special symbols', () => {
    expect(containsSpecSymbol("Az2sssA")).toEqual(false);
    expect(containsSpecSymbol("!")).toEqual(true);
    expect(containsSpecSymbol("~")).toEqual(true);
    expect(containsSpecSymbol("`")).toEqual(true);
    expect(containsSpecSymbol("@")).toEqual(true);
    expect(containsSpecSymbol("#")).toEqual(true);
    expect(containsSpecSymbol("$")).toEqual(true);
    expect(containsSpecSymbol("%")).toEqual(true);
    expect(containsSpecSymbol("^")).toEqual(true);
    expect(containsSpecSymbol("&")).toEqual(true);
    expect(containsSpecSymbol("*")).toEqual(true);
    expect(containsSpecSymbol("(")).toEqual(true);
    expect(containsSpecSymbol(")")).toEqual(true);
    expect(containsSpecSymbol("-")).toEqual(true);
    expect(containsSpecSymbol("_")).toEqual(true);
    expect(containsSpecSymbol("+")).toEqual(true);
    expect(containsSpecSymbol("=")).toEqual(true);
    expect(containsSpecSymbol("/")).toEqual(true);
    expect(containsSpecSymbol("?")).toEqual(true);
    expect(containsSpecSymbol("'")).toEqual(true);
    expect(containsSpecSymbol("\"")).toEqual(true);
    expect(containsSpecSymbol(":")).toEqual(true);
    expect(containsSpecSymbol(";")).toEqual(true);
    expect(containsSpecSymbol(">")).toEqual(true);
    expect(containsSpecSymbol(".")).toEqual(true);
    expect(containsSpecSymbol("<")).toEqual(true);
    expect(containsSpecSymbol(",")).toEqual(true);
});