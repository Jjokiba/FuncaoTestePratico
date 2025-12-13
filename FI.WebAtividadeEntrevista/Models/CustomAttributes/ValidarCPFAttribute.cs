using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

public class ValidarCPFAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null)
            return ValidationResult.Success;

        if(!Regex.IsMatch(value.ToString(), @"[0-9]{3}\.[0-9]{3}\.[0-9]{3}\-[0-9]{2}"))
            return new ValidationResult("Digite um CPF válido");

        var numeros = Regex.Replace(value.ToString(), @"[^0-9]", "");
        if (!validarCPF(numeros))
            return new ValidationResult("CPF informado não existe");

        return ValidationResult.Success;
    }

    /// <summary>
    ///     Função para aplicar a regra de validação do CPF
    /// </summary>
    /// <param name="numeros">String com todos os numeros do CPF</param>
    /// <returns>bool IsValid</returns>
    private bool validarCPF(string numeros)
    {
        int[] array = numeros.Select(c => int.Parse(c.ToString())).ToArray();

        int sum = 0;
        for (int i = 0; i < 9; i++)
            sum += array[i] * (10 - i);

        int result = (sum * 10) % 11;
        if (result == 10) result = 0;
        if (array[9] != result)
            return false;

        sum = 0;
        for (int i = 0; i < 10; i++)
            sum += array[i] * (11 - i);

        result = (sum * 10) % 11;
        if (result == 10) result = 0;

        return array[10] == result;
    }
}
