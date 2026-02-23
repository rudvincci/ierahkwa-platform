{{/*
Return the fully qualified name for the infra chart.
*/}}
{{- define "infra.fullname" -}}
{{- printf "%s-%s" .Release.Name "infra" | trunc 63 | trimSuffix "-" -}}
{{- end -}}
