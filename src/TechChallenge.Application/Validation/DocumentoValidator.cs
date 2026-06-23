namespace TechChallenge.Application.Validation;

public static class DocumentoValidator
{
    public static bool CpfValido(string cpf)
    {
        var digits = ApenasDigitos(cpf);
        if (digits.Length != 11)
            return false;

        if (digits.All(digit => digit == digits[0]))
            return false;

        var primeiroDigito = CalcularDigito(digits[..9], 10);
        var segundoDigito = CalcularDigito(digits[..10], 11);

        return digits[9] == primeiroDigito && digits[10] == segundoDigito;
    }

    private static string ApenasDigitos(string value)
    {
        return new string(value.Where(char.IsDigit).ToArray());
    }

    private static char CalcularDigito(string digits, int pesoInicial)
    {
        var soma = digits
            .Select((digit, index) => (digit - '0') * (pesoInicial - index))
            .Sum();

        var resto = soma % 11;
        var digito = resto < 2 ? 0 : 11 - resto;

        return (char)('0' + digito);
    }
}
