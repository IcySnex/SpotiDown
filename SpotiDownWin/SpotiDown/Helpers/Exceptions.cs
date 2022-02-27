using System;

namespace SpotiDown.Helpers;

public class Exceptions
{
    public static Exception JsonInvalid = new("Deserialized JSON returned invalid type or null.");
}