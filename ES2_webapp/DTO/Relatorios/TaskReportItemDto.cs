using System;
using System.Text.Json.Serialization;

namespace ES2_webapp.DTO.Relatorios
{
    public class TaskReportItemDto
    {
        [JsonInclude]
        public DateTime Data { get; set; }
        [JsonInclude]
        public string NomeProjeto { get; set; }
        [JsonInclude]
        public string NomeTarefa { get; set; }
        [JsonInclude]
        public decimal Horas { get; set; }
        [JsonInclude]
        public decimal Custo { get; set; }
    }
}