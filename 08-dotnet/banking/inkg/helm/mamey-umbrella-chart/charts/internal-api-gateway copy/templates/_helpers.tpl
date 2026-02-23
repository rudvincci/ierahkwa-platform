{{/*
Return the fully qualified name for the internal-api-gateway chart.
*/}}
{{- define "internal-api-gateway.fullname" -}}
{{- printf "%s-%s" .Release.Name "internal-api-gateway" | trunc 63 | trimSuffix "-" -}}
{{- end -}}
