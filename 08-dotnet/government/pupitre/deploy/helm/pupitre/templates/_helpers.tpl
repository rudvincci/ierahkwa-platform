{{/*
Expand the name of the chart.
*/}}
{{- define "pupitre.name" -}}
{{- default .Chart.Name .Values.nameOverride | trunc 63 | trimSuffix "-" }}
{{- end }}

{{/*
Create a default fully qualified app name.
*/}}
{{- define "pupitre.fullname" -}}
{{- if .Values.fullnameOverride }}
{{- .Values.fullnameOverride | trunc 63 | trimSuffix "-" }}
{{- else }}
{{- $name := default .Chart.Name .Values.nameOverride }}
{{- if contains $name .Release.Name }}
{{- .Release.Name | trunc 63 | trimSuffix "-" }}
{{- else }}
{{- printf "%s-%s" .Release.Name $name | trunc 63 | trimSuffix "-" }}
{{- end }}
{{- end }}
{{- end }}

{{/*
Create chart name and version as used by the chart label.
*/}}
{{- define "pupitre.chart" -}}
{{- printf "%s-%s" .Chart.Name .Chart.Version | replace "+" "_" | trunc 63 | trimSuffix "-" }}
{{- end }}

{{/*
Common labels
*/}}
{{- define "pupitre.labels" -}}
helm.sh/chart: {{ include "pupitre.chart" . }}
{{ include "pupitre.selectorLabels" . }}
{{- if .Chart.AppVersion }}
app.kubernetes.io/version: {{ .Chart.AppVersion | quote }}
{{- end }}
app.kubernetes.io/managed-by: {{ .Release.Service }}
{{- end }}

{{/*
Selector labels
*/}}
{{- define "pupitre.selectorLabels" -}}
app.kubernetes.io/name: {{ include "pupitre.name" . }}
app.kubernetes.io/instance: {{ .Release.Name }}
{{- end }}

{{/*
Create the name of the service account to use
*/}}
{{- define "pupitre.serviceAccountName" -}}
{{- if .Values.serviceAccount.create }}
{{- default (include "pupitre.fullname" .) .Values.serviceAccount.name }}
{{- else }}
{{- default "default" .Values.serviceAccount.name }}
{{- end }}
{{- end }}

{{/*
Service template for microservices
*/}}
{{- define "pupitre.service" -}}
apiVersion: v1
kind: Service
metadata:
  name: {{ .name }}
  namespace: {{ .namespace }}
  labels:
    {{- include "pupitre.labels" .context | nindent 4 }}
    app.kubernetes.io/component: {{ .name }}
spec:
  type: ClusterIP
  ports:
    - port: {{ .port }}
      targetPort: {{ .port }}
      protocol: TCP
      name: http
  selector:
    app.kubernetes.io/component: {{ .name }}
{{- end }}

{{/*
Deployment template for microservices
*/}}
{{- define "pupitre.deployment" -}}
apiVersion: apps/v1
kind: Deployment
metadata:
  name: {{ .name }}
  namespace: {{ .namespace }}
  labels:
    {{- include "pupitre.labels" .context | nindent 4 }}
    app.kubernetes.io/component: {{ .name }}
spec:
  replicas: {{ .replicaCount }}
  selector:
    matchLabels:
      app.kubernetes.io/component: {{ .name }}
  template:
    metadata:
      labels:
        app.kubernetes.io/component: {{ .name }}
    spec:
      serviceAccountName: {{ include "pupitre.serviceAccountName" .context }}
      securityContext:
        {{- toYaml .context.Values.podSecurityContext | nindent 8 }}
      containers:
        - name: {{ .name }}
          securityContext:
            {{- toYaml .context.Values.securityContext | nindent 12 }}
          image: "{{ .context.Values.global.imageRegistry }}/{{ .image.repository }}:{{ .image.tag }}"
          imagePullPolicy: {{ .image.pullPolicy | default "IfNotPresent" }}
          ports:
            - name: http
              containerPort: {{ .port }}
              protocol: TCP
          livenessProbe:
            httpGet:
              path: /health/live
              port: http
            initialDelaySeconds: 30
            periodSeconds: 10
          readinessProbe:
            httpGet:
              path: /health/ready
              port: http
            initialDelaySeconds: 10
            periodSeconds: 5
          resources:
            {{- toYaml .resources | nindent 12 }}
          env:
            - name: ASPNETCORE_ENVIRONMENT
              value: "Production"
            - name: ASPNETCORE_URLS
              value: "http://+:{{ .port }}"
{{- end }}
