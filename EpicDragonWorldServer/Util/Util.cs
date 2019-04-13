﻿using System.Globalization;

/**
 * Author: Pantelis Andrianakis
 * Date: November 7th 2018
 */
class Util
{
    public static char[] ILLEGAL_CHARACTERS =
{
        '/',
        '\n',
        '\r',
        '\t',
        '\0',
        '\f',
        '`',
        '?',
        '*',
        '\\',
        '<',
        '>',
        '|',
        '\"',
        '{',
        '}',
        '(',
        ')'
    };

    public static int HexStringToInt(string hex)
    {
        return int.Parse(hex, NumberStyles.HexNumber);
    }
}
