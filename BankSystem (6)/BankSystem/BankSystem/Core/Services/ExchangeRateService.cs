using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace BankSystem.Core.Services
{
    /// <summary>
    /// Сервис для получения актуального обменного курса между двумя валютами.
    /// Использует официальный JSON API Национального Банка Республики Беларусь.
    /// </summary>
    public class ExchangeRateService
    {
        private static readonly HttpClient _client = new HttpClient();

        /// <summary>
        /// Получить курс обмена между двумя валютами (например, BYN → USD).
        /// </summary>
        /// <param name="from">Код валюты, из которой конвертируем (например, "BYN", "USD", "EUR", "RUB").</param>
        /// <param name="to">Код валюты, в которую конвертируем.</param>
        /// <returns>Курс: сколько единиц валюты "from" за одну "to".</returns>
        public async Task<decimal> GetExchangeRateAsync(string from, string to)
        {
            if (from == to) return 1m;

            decimal fromRate = from == "BYN" ? 1m : await GetBynRateAsync(from);
            decimal toRate = to == "BYN" ? 1m : await GetBynRateAsync(to);

            return fromRate / toRate;
        }

        /// <summary>
        /// Получить курс указанной валюты к BYN (по данным НБРБ).
        /// </summary>
        private async Task<decimal> GetBynRateAsync(string currency)
        {
            string url = $"https://api.nbrb.by/exrates/rates/{currency}?parammode=2";
            using var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            var rate = doc.RootElement.GetProperty("Cur_OfficialRate").GetDecimal();
            var scale = doc.RootElement.GetProperty("Cur_Scale").GetInt32();

            return rate / scale;
        }
    }
}