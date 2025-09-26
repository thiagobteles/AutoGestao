namespace AutoGestao.Extensions
{
    public static class StringExtensions
    {
        public static string ToSnakeCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            var startUnderscores = System.Text.RegularExpressions.Regex.Match(input, @"^_+");
            return startUnderscores + System.Text.RegularExpressions.Regex.Replace(input, @"([a-z0-9])([A-Z])", "$1_$2").ToLower();
        }

        public static bool CpfValido(this string cpf)
        {
            if (string.IsNullOrEmpty(cpf) || cpf.Length != 11)
            {
                return false;
            }

            // Verificar se todos os dígitos são iguais
            if (cpf.All(c => c == cpf[0]))
            {
                return false;
            }

            // Validar dígitos verificadores
            var numbers = cpf.Select(c => int.Parse(c.ToString())).ToArray();

            var sum = 0;
            for (var i = 0; i < 9; i++)
            {
                sum += numbers[i] * (10 - i);
            }

            var remainder = sum % 11;
            var digit1 = remainder < 2 ? 0 : 11 - remainder;

            if (numbers[9] != digit1)
            {
                return false;
            }

            sum = 0;
            for (var i = 0; i < 10; i++)
            {
                sum += numbers[i] * (11 - i);
            }

            remainder = sum % 11;
            var digit2 = remainder < 2 ? 0 : 11 - remainder;

            return numbers[10] == digit2;
        }

        public static bool CnpjValido(this string cnpj)
        {
            if (string.IsNullOrEmpty(cnpj) || cnpj.Length != 14)
            {
                return false;
            }

            // Verificar se todos os dígitos são iguais
            if (cnpj.All(c => c == cnpj[0]))
            {
                return false;
            }

            var numbers = cnpj.Select(c => int.Parse(c.ToString())).ToArray();

            // Primeiro dígito verificador
            var sequence1 = new int[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            var sum = 0;
            for (var i = 0; i < 12; i++)
            {
                sum += numbers[i] * sequence1[i];
            }

            var remainder = sum % 11;
            var digit1 = remainder < 2 ? 0 : 11 - remainder;

            if (numbers[12] != digit1)
            {
                return false;
            }

            // Segundo dígito verificador
            var sequence2 = new int[] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            sum = 0;
            for (var i = 0; i < 13; i++)
            {
                sum += numbers[i] * sequence2[i];
            }

            remainder = sum % 11;
            var digit2 = remainder < 2 ? 0 : 11 - remainder;

            return numbers[13] == digit2;
        }

        /// <summary>
        /// Aplica máscara de CPF (000.000.000-00) a uma string que contenha 11 dígitos.
        /// Se o input não tiver 11 dígitos, retorna o input original (após remover espaços).
        /// </summary>
        public static string AplicarMascaraCpf(this string input)
        {
            if (input is null)
            {
                return null;
            }

            var digits = new string([.. input.Where(char.IsDigit)]);
            return digits.Length != 11
                ? input.Trim()
                : $"{digits.Substring(0, 3)}.{digits.Substring(3, 3)}.{digits.Substring(6, 3)}-{digits.Substring(9, 2)}";
        }

        /// <summary>
        /// Aplica máscara de CNPJ (00.000.000/0000-00) a uma string que contenha 14 dígitos.
        /// Se o input não tiver 14 dígitos, retorna o input original (após remover espaços).
        /// </summary>
        public static string AplicarMascaraCnpj(this string input)
        {
            if (input is null)
            {
                return null;
            }

            var digits = new string([.. input.Where(char.IsDigit)]);
            return digits.Length != 14
                ? input.Trim()
                : $"{digits.Substring(0, 2)}.{digits.Substring(2, 3)}.{digits.Substring(5, 3)}/{digits.Substring(8, 4)}-{digits.Substring(12, 2)}";
        }
    }
}
