using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Iwp.Services.RabbitMQConsumerForBlob.Dtos
{
    public class DocumentRequestDTO
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Content { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("CaseNum")]
        public string CaseNum { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("ContentType")]
        public string ContentType { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string FileName { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string FilePath { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Status { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string FormattedContent { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string TempFormattedContent { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string TempPlainContent { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime ServiceDate { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CourtName { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string[] Plaintiffs { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string[] Defendants { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public string _Id { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ParentId { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? ContentItemNum { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public SimilarityDto[] SelfGroupingLsa { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<List<object>> ContentCoords { get; set; }

    }
    public class DocumentRequests:ICollection<DocumentRequestDTO>
    {
        public static DocumentRequestDTO[] DocRequests { get; set; }

        public int Count { get; set; }

        public bool IsReadOnly { get; set; }

        public void Add(DocumentRequestDTO item)
        {
        }

        public void Clear()
        {
        }

        public bool Contains(DocumentRequestDTO item)
        {
            return false;
        }

        public void CopyTo(DocumentRequestDTO[] array, int arrayIndex)
        {

        }

        public IEnumerator<DocumentRequestDTO> GetEnumerator()
        {
            return null;
        }

        public bool Remove(DocumentRequestDTO item)
        {
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return null;
        }
    }
    public class SimilarityDto
    {
        public decimal Similarity { get; set; }
        public string Id { get; set; }
    }
}
