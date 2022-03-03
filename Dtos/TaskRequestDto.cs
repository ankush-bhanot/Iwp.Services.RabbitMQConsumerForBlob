// Copyright © 2021 Noetic Analytics LLC.  All rights reserved 
//  Author: Hitesh Kumar 
//  File: TaskRequestDto.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Iwp.Services.RabbitMQConsumerForBlob.Dtos
{
    public class TaskRequestDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("owner_name")]
        public string OwnerName { get; set; }

        [JsonPropertyName("owner_id")]
        public Guid? Owner { get; set; }

        [JsonPropertyName("case_id")]
        public string CaseId { get; set; }

        [JsonPropertyName("task_type")]
        public int TaskType { get; set; }

        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("parent_document_id")]
        public string ParentDocumentId { get; set; }

        [JsonPropertyName("question_id")]
        public string QuestionId { get; set; }

        [JsonPropertyName("temp_Response")]
        public string TempResponse { get; set; }

        [JsonPropertyName("due_Date")]
        public DateTime? DueDate { get; set; }

        [JsonPropertyName("request_id")]
        public string RequestID { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}
