{{/*
Return the fully qualified name for the microservices chart.
*/}}
{{- define "microservices.fullname" -}}
{{- printf "%s-%s" .Release.Name "microservices" | trunc 63 | trimSuffix "-" -}}
{{- end -}}
