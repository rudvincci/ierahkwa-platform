{{/*
Return the fully qualified name for the api-gateway chart.
*/}}
{{- define "api-gateway.fullname" -}}
{{- printf "%s-%s" .Release.Name "api-gateway" | trunc 63 | trimSuffix "-" -}}
{{- end -}}
