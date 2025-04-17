namespace Api.Host.Contracts.V1;

[MimeType("audio-upload")]
public record AudioUpload(string[] BlockIds);
