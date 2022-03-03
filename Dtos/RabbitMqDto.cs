// Copyright © 2021 Noetic Analytics LLC.  All rights reserved 
//  Author: Tejinder Singh 
//  File: RabbitMqDto.cs

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Iwp.Services.RabbitMQConsumerForBlob.Dtos
{
    public class RabbitMqMessageDto
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }
        [JsonPropertyName("paras_raw_data")]
        public string Payload { get; set; }
        [JsonPropertyName("execution_date_time")]
        public ExecutionDateTimeStamp ExecutionDateTimeStamp { get; set; }
        [JsonPropertyName("file_meta_data")]
        public FileDto FileDto { get; set; }
        [JsonPropertyName("virus_found")]
        public List<string> Viruses { get; set; }
        [JsonPropertyName("message_type")]
        public Guid MessageType { get; set; }
    }

    public class ExecutionDateTimeStamp
    {
        [JsonPropertyName("start_date_time")]
        public DateTime StartDate { get; set; }
        [JsonPropertyName("end_date_time")]
        public DateTime EndDate { get; set; }
    }

    public class FileDto
    {
        [JsonPropertyName("document_id")]
        public string DocumentID { get; set; }

        [JsonPropertyName("client_id")]
        public Guid ClientId { get; set; }
            
        [JsonPropertyName("client_storage")]
        public string ClientStorage { get; set; }

        [JsonPropertyName("file_name")]
        public string FileName { get; set; }

        [JsonPropertyName("case_id")]
        public string CaseID { get; set; }

        [JsonPropertyName("case_name")]
        public string CaseName { get; set; }

        [JsonPropertyName("file_hash")]
        public string FileHash { get; set; }

    }

}
