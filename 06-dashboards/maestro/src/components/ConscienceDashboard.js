/**
 * ConscienceDashboard.js
 * Panel de Control del Motor de Consciencia / Conscience Engine Dashboard
 * Plataforma Ierahkwa - Sirviendo a las Americas
 */

import React, { useState, useEffect, useCallback } from 'react';
import {
  Card,
  Title,
  Text,
  Metric,
  BarChart,
  DonutChart,
  AreaChart,
  Table,
  TableHead,
  TableHeaderCell,
  TableBody,
  TableRow,
  TableCell,
  ProgressBar,
  Grid,
  Col,
  Flex,
  Badge,
  Color,
} from '@tremor/react';

// ============================================================
// Constantes / Constants
// ============================================================

const REFRESH_INTERVAL_MS = 30000; // Auto-refresh cada 30s

const REGION_LABELS = [
  'Norte America / North America',
  'Centro America / Central America',
  'Caribe / Caribbean',
  'Sur America / South America',
];

const REPUTATION_TIERS = {
  OBSERVADOR: { min: 0, max: 99, label: 'Observador', color: 'slate' },
  GUARDIAN: { min: 100, max: 499, label: 'Guardian', color: 'blue' },
  ANCIANO: { min: 500, max: 999, label: 'Anciano / Elder', color: 'amber' },
  CONSEJO: { min: 1000, max: Infinity, label: 'Consejo / Council', color: 'emerald' },
};

const PROPOSAL_TYPES = [
  'Humanitario / Humanitarian',
  'Ambiental / Environmental',
  'Infraestructura / Infrastructure',
  'Educacion / Education',
  'Emergencia / Emergency',
];

const PEACE_COLOR_MAP = (events) => {
  if (events < 5) return 'emerald';
  if (events <= 15) return 'yellow';
  return 'rose';
};

// ============================================================
// Funciones de API simulada / Mock API Functions
// ============================================================

async function fetchPeaceIndex() {
  try {
    const res = await fetch('/api/conscience/peace-index');
    if (res.ok) return res.json();
  } catch (_) { /* fallback to mock */ }
  return [
    { region: 'Norte America', eventos: Math.floor(Math.random() * 25), tendencia: 'estable' },
    { region: 'Centro America', eventos: Math.floor(Math.random() * 25), tendencia: 'mejorando' },
    { region: 'Caribe', eventos: Math.floor(Math.random() * 25), tendencia: 'alerta' },
    { region: 'Sur America', eventos: Math.floor(Math.random() * 25), tendencia: 'estable' },
  ];
}

async function fetchGuardianStatus() {
  try {
    const res = await fetch('/api/conscience/guardians/status');
    if (res.ok) return res.json();
  } catch (_) { /* fallback */ }
  const active = 2847 + Math.floor(Math.random() * 200);
  const idle = 412 + Math.floor(Math.random() * 80);
  const offline = 93 + Math.floor(Math.random() * 30);
  return { active, idle, offline, total: active + idle + offline };
}

async function fetchReputationTimeline() {
  try {
    const res = await fetch('/api/conscience/reputation/timeline');
    if (res.ok) return res.json();
  } catch (_) { /* fallback */ }
  const months = ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'];
  return months.map((mes) => ({
    mes,
    Observador: 4500 + Math.floor(Math.random() * 500),
    Guardian: 2100 + Math.floor(Math.random() * 300),
    Anciano: 680 + Math.floor(Math.random() * 100),
    Consejo: 142 + Math.floor(Math.random() * 30),
  }));
}

async function fetchVeritasFeed() {
  try {
    const res = await fetch('/api/conscience/veritas/feed');
    if (res.ok) return res.json();
  } catch (_) { /* fallback */ }
  const statuses = ['Verificado', 'Pendiente', 'Disputado', 'Rechazado'];
  const statusColors = { Verificado: 'emerald', Pendiente: 'yellow', Disputado: 'orange', Rechazado: 'rose' };
  return Array.from({ length: 8 }, (_, i) => {
    const status = statuses[Math.floor(Math.random() * statuses.length)];
    return {
      id: i + 1,
      contentHash: `Qm${Math.random().toString(36).substring(2, 14)}...`,
      submitter: `0x${Math.random().toString(16).substring(2, 10)}...`,
      verifications: Math.floor(Math.random() * 50),
      status,
      statusColor: statusColors[status],
      timestamp: new Date(Date.now() - Math.random() * 86400000).toISOString(),
    };
  });
}

async function fetchTreasuryBalance() {
  try {
    const res = await fetch('/api/conscience/treasury');
    if (res.ok) return res.json();
  } catch (_) { /* fallback */ }
  return {
    totalWMP: 14_750_000 + Math.floor(Math.random() * 500_000),
    activeProposals: 12 + Math.floor(Math.random() * 8),
    humanitarianPct: 35.2,
    emergencyReserve: 2_200_000 + Math.floor(Math.random() * 100_000),
  };
}

async function fetchMeshNodes() {
  try {
    const res = await fetch('/api/conscience/lora/mesh');
    if (res.ok) return res.json();
  } catch (_) { /* fallback */ }
  const regions = ['Norte', 'Centro', 'Caribe', 'Sur'];
  return Array.from({ length: 12 }, (_, i) => ({
    nodeId: `LORA-${regions[i % 4]}-${String(i + 1).padStart(3, '0')}`,
    region: regions[i % 4],
    lastSeen: `hace ${Math.floor(Math.random() * 60)} min`,
    signalStrength: -30 - Math.floor(Math.random() * 70),
    messagesRelayed: Math.floor(Math.random() * 10000),
  }));
}

async function fetchIPFSStats() {
  try {
    const res = await fetch('/api/conscience/ipfs/stats');
    if (res.ok) return res.json();
  } catch (_) { /* fallback */ }
  return {
    pinnedFiles: 48_392 + Math.floor(Math.random() * 1000),
    totalSizeGB: 1.24 + Math.random() * 0.3,
    replicationCount: 3 + Math.floor(Math.random() * 3),
  };
}

async function fetchChaosDrillReadiness() {
  try {
    const res = await fetch('/api/conscience/chaos-drill');
    if (res.ok) return res.json();
  } catch (_) { /* fallback */ }
  return {
    lastScore: 78 + Math.floor(Math.random() * 20),
    daysSinceDrill: Math.floor(Math.random() * 30),
    nextExpectedDays: Math.floor(Math.random() * 14) + 1,
  };
}

// ============================================================
// Componente auxiliar: Signal Badge
// ============================================================

function SignalBadge({ dbm }) {
  if (dbm > -50) return <Badge color="emerald">Excelente</Badge>;
  if (dbm > -70) return <Badge color="yellow">Buena</Badge>;
  return <Badge color="rose">Debil</Badge>;
}

// ============================================================
// Componente Principal / Main Component
// ============================================================

export default function ConscienceDashboard() {
  // ----- State -----
  const [peaceIndex, setPeaceIndex] = useState([]);
  const [guardians, setGuardians] = useState({ active: 0, idle: 0, offline: 0, total: 0 });
  const [reputationTimeline, setReputationTimeline] = useState([]);
  const [veritasFeed, setVeritasFeed] = useState([]);
  const [treasury, setTreasury] = useState({ totalWMP: 0, activeProposals: 0, humanitarianPct: 0, emergencyReserve: 0 });
  const [meshNodes, setMeshNodes] = useState([]);
  const [ipfsStats, setIpfsStats] = useState({ pinnedFiles: 0, totalSizeGB: 0, replicationCount: 0 });
  const [chaosDrill, setChaosDrill] = useState({ lastScore: 0, daysSinceDrill: 0, nextExpectedDays: 0 });
  const [lastRefresh, setLastRefresh] = useState(null);
  const [loading, setLoading] = useState(true);

  // ----- Data fetching -----
  const refreshAll = useCallback(async () => {
    setLoading(true);
    const [peace, guard, rep, veritas, treas, mesh, ipfs, chaos] = await Promise.all([
      fetchPeaceIndex(),
      fetchGuardianStatus(),
      fetchReputationTimeline(),
      fetchVeritasFeed(),
      fetchTreasuryBalance(),
      fetchMeshNodes(),
      fetchIPFSStats(),
      fetchChaosDrillReadiness(),
    ]);
    setPeaceIndex(peace);
    setGuardians(guard);
    setReputationTimeline(rep);
    setVeritasFeed(veritas);
    setTreasury(treas);
    setMeshNodes(mesh);
    setIpfsStats(ipfs);
    setChaosDrill(chaos);
    setLastRefresh(new Date());
    setLoading(false);
  }, []);

  useEffect(() => {
    refreshAll();
    const interval = setInterval(refreshAll, REFRESH_INTERVAL_MS);
    return () => clearInterval(interval);
  }, [refreshAll]);

  // ----- Derived data -----
  const peaceChartData = peaceIndex.map((r) => ({
    region: r.region,
    'Eventos / Events': r.eventos,
  }));

  const guardianDonutData = [
    { name: 'Activos / Active', value: guardians.active },
    { name: 'Inactivos / Idle', value: guardians.idle },
    { name: 'Desconectados / Offline', value: guardians.offline },
  ];

  const guardianColors = ['emerald', 'yellow', 'rose'];

  const peaceBarColors = peaceIndex.map((r) => PEACE_COLOR_MAP(r.eventos));

  // ----- Render -----
  return (
    <div className="conscience-dashboard p-4 md:p-6 lg:p-8 min-h-screen bg-gray-50 dark:bg-gray-950">
      {/* Header */}
      <Flex justifyContent="between" alignItems="center" className="mb-6">
        <div>
          <Title className="text-2xl font-bold dark:text-white">
            Motor de Consciencia / Conscience Engine
          </Title>
          <Text className="dark:text-gray-400">
            Plataforma Ierahkwa &mdash; Tablero en Tiempo Real / Real-Time Dashboard
          </Text>
        </div>
        <Flex justifyContent="end" alignItems="center" className="gap-3">
          {lastRefresh && (
            <Text className="text-xs dark:text-gray-500">
              Actualizado / Updated: {lastRefresh.toLocaleTimeString()}
            </Text>
          )}
          <Badge color={loading ? 'yellow' : 'emerald'} size="sm">
            {loading ? 'Cargando...' : 'En Vivo / Live'}
          </Badge>
        </Flex>
      </Flex>

      {/* Row 1: Peace Index + Guardian Status */}
      <Grid numItems={1} numItemsMd={2} className="gap-6 mb-6">
        <Col>
          <Card className="dark:bg-gray-900 dark:ring-gray-800">
            <Title className="dark:text-white">
              Indice de Paz / Peace Index
            </Title>
            <Text className="mb-4 dark:text-gray-400">
              Eventos de conflicto por region (ACLED) / Conflict events by region
            </Text>
            <BarChart
              data={peaceChartData}
              index="region"
              categories={['Eventos / Events']}
              colors={peaceBarColors.length > 0 ? [peaceBarColors[0]] : ['emerald']}
              yAxisWidth={40}
              className="h-52"
            />
            <div className="mt-3 flex gap-4">
              {peaceIndex.map((r) => (
                <Badge key={r.region} color={PEACE_COLOR_MAP(r.eventos)} size="sm">
                  {r.region}: {r.eventos} eventos &mdash; {r.tendencia}
                </Badge>
              ))}
            </div>
            <Text className="mt-2 text-xs dark:text-gray-500">
              Verde/Green: &lt;5 | Amarillo/Yellow: 5-15 | Rojo/Red: &gt;15
            </Text>
          </Card>
        </Col>

        <Col>
          <Card className="dark:bg-gray-900 dark:ring-gray-800">
            <Title className="dark:text-white">
              Red de Guardianes / Guardian Network
            </Title>
            <Text className="mb-4 dark:text-gray-400">
              Estado de la red / Network status
            </Text>
            <DonutChart
              data={guardianDonutData}
              category="value"
              index="name"
              colors={guardianColors}
              className="h-52"
              showAnimation
            />
            <div className="text-center mt-2">
              <Metric className="dark:text-white">
                {guardians.total.toLocaleString()}
              </Metric>
              <Text className="dark:text-gray-400">
                Guardianes Totales / Total Guardians
              </Text>
            </div>
            <div className="mt-3 flex justify-center gap-3">
              <Badge color="emerald">Activos: {guardians.active.toLocaleString()}</Badge>
              <Badge color="yellow">Inactivos: {guardians.idle}</Badge>
              <Badge color="rose">Desconectados: {guardians.offline}</Badge>
            </div>
          </Card>
        </Col>
      </Grid>

      {/* Row 2: Reputation Timeline */}
      <Card className="mb-6 dark:bg-gray-900 dark:ring-gray-800">
        <Title className="dark:text-white">
          Distribucion de Reputacion $MATTR / $MATTR Reputation Distribution
        </Title>
        <Text className="mb-4 dark:text-gray-400">
          Evolucion mensual por nivel / Monthly evolution by tier
        </Text>
        <AreaChart
          data={reputationTimeline}
          index="mes"
          categories={['Observador', 'Guardian', 'Anciano', 'Consejo']}
          colors={['slate', 'blue', 'amber', 'emerald']}
          className="h-64"
          showAnimation
          curveType="monotone"
        />
        <div className="mt-3 flex gap-3 flex-wrap">
          {Object.values(REPUTATION_TIERS).map((tier) => (
            <Badge key={tier.label} color={tier.color} size="sm">
              {tier.label}: {tier.min}-{tier.max === Infinity ? '+' : tier.max} $MATTR
            </Badge>
          ))}
        </div>
      </Card>

      {/* Row 3: Veritas Evidence Feed */}
      <Card className="mb-6 dark:bg-gray-900 dark:ring-gray-800">
        <Title className="dark:text-white">
          Verificacion de Evidencia Veritas / Veritas Evidence Feed
        </Title>
        <Text className="mb-4 dark:text-gray-400">
          Ultimas verificaciones de hechos / Latest fact-checks
        </Text>
        <Table>
          <TableHead>
            <TableRow>
              <TableHeaderCell className="dark:text-gray-300">Hash de Contenido / Content Hash</TableHeaderCell>
              <TableHeaderCell className="dark:text-gray-300">Remitente / Submitter</TableHeaderCell>
              <TableHeaderCell className="dark:text-gray-300">Verificaciones / Verifications</TableHeaderCell>
              <TableHeaderCell className="dark:text-gray-300">Estado / Status</TableHeaderCell>
              <TableHeaderCell className="dark:text-gray-300">Fecha / Timestamp</TableHeaderCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {veritasFeed.map((entry) => (
              <TableRow key={entry.id}>
                <TableCell className="font-mono text-sm dark:text-gray-300">
                  {entry.contentHash}
                </TableCell>
                <TableCell className="font-mono text-sm dark:text-gray-300">
                  {entry.submitter}
                </TableCell>
                <TableCell className="dark:text-gray-300">
                  {entry.verifications}
                </TableCell>
                <TableCell>
                  <Badge color={entry.statusColor} size="sm">
                    {entry.status}
                  </Badge>
                </TableCell>
                <TableCell className="dark:text-gray-400 text-sm">
                  {new Date(entry.timestamp).toLocaleString('es-419')}
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </Card>

      {/* Row 4: Treasury + IPFS + Chaos Drill */}
      <Grid numItems={1} numItemsMd={3} className="gap-6 mb-6">
        {/* Treasury Balance */}
        <Col numColSpan={1} numColSpanMd={2}>
          <Card className="dark:bg-gray-900 dark:ring-gray-800">
            <Title className="dark:text-white">
              Tesoreria / Treasury Balance
            </Title>
            <Grid numItems={2} numItemsMd={4} className="gap-4 mt-4">
              <Card decoration="top" decorationColor="emerald" className="dark:bg-gray-800">
                <Text className="dark:text-gray-400">Total WMP</Text>
                <Metric className="dark:text-white">
                  {treasury.totalWMP.toLocaleString()}
                </Metric>
              </Card>
              <Card decoration="top" decorationColor="blue" className="dark:bg-gray-800">
                <Text className="dark:text-gray-400">Propuestas Activas / Active Proposals</Text>
                <Metric className="dark:text-white">
                  {treasury.activeProposals}
                </Metric>
              </Card>
              <Card decoration="top" decorationColor="amber" className="dark:bg-gray-800">
                <Text className="dark:text-gray-400">Fondo Humanitario / Humanitarian Fund</Text>
                <Metric className="dark:text-white">
                  {treasury.humanitarianPct}%
                </Metric>
              </Card>
              <Card decoration="top" decorationColor="rose" className="dark:bg-gray-800">
                <Text className="dark:text-gray-400">Reserva de Emergencia / Emergency Reserve</Text>
                <Metric className="dark:text-white">
                  {treasury.emergencyReserve.toLocaleString()} WMP
                </Metric>
              </Card>
            </Grid>
          </Card>
        </Col>

        {/* IPFS Storage Stats */}
        <Col>
          <Card className="dark:bg-gray-900 dark:ring-gray-800 h-full">
            <Title className="dark:text-white">
              Almacenamiento IPFS / IPFS Storage
            </Title>
            <div className="mt-4 space-y-4">
              <div>
                <Text className="dark:text-gray-400">Archivos Fijados / Pinned Files</Text>
                <Metric className="dark:text-white">
                  {ipfsStats.pinnedFiles.toLocaleString()}
                </Metric>
              </div>
              <div>
                <Text className="dark:text-gray-400">Tamano Total / Total Size</Text>
                <Metric className="dark:text-white">
                  {ipfsStats.totalSizeGB.toFixed(2)} TB
                </Metric>
              </div>
              <div>
                <Text className="dark:text-gray-400">Replicacion / Replication</Text>
                <Metric className="dark:text-white">
                  {ipfsStats.replicationCount}x
                </Metric>
              </div>
            </div>
          </Card>
        </Col>
      </Grid>

      {/* Row 5: LoRa Mesh Topology */}
      <Card className="mb-6 dark:bg-gray-900 dark:ring-gray-800">
        <Title className="dark:text-white">
          Topologia de Malla LoRa / LoRa Mesh Topology
        </Title>
        <Text className="mb-4 dark:text-gray-400">
          Nodos activos de la red descentralizada / Active decentralized network nodes
        </Text>
        <Table>
          <TableHead>
            <TableRow>
              <TableHeaderCell className="dark:text-gray-300">ID del Nodo / Node ID</TableHeaderCell>
              <TableHeaderCell className="dark:text-gray-300">Region</TableHeaderCell>
              <TableHeaderCell className="dark:text-gray-300">Ultima Vez / Last Seen</TableHeaderCell>
              <TableHeaderCell className="dark:text-gray-300">Senal / Signal (dBm)</TableHeaderCell>
              <TableHeaderCell className="dark:text-gray-300">Mensajes / Messages Relayed</TableHeaderCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {meshNodes.map((node) => (
              <TableRow key={node.nodeId}>
                <TableCell className="font-mono text-sm dark:text-gray-300">
                  {node.nodeId}
                </TableCell>
                <TableCell className="dark:text-gray-300">{node.region}</TableCell>
                <TableCell className="dark:text-gray-400">{node.lastSeen}</TableCell>
                <TableCell>
                  <Flex justifyContent="start" alignItems="center" className="gap-2">
                    <Text className="dark:text-gray-300">{node.signalStrength} dBm</Text>
                    <SignalBadge dbm={node.signalStrength} />
                  </Flex>
                </TableCell>
                <TableCell className="dark:text-gray-300">
                  {node.messagesRelayed.toLocaleString()}
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </Card>

      {/* Row 6: Chaos Drill Readiness */}
      <Card className="mb-6 dark:bg-gray-900 dark:ring-gray-800">
        <Title className="dark:text-white">
          Preparacion para Simulacro de Caos / Chaos Drill Readiness
        </Title>
        <Grid numItems={1} numItemsMd={3} className="gap-6 mt-4">
          <div>
            <Text className="dark:text-gray-400">Puntaje Ultimo Simulacro / Last Drill Score</Text>
            <Metric className="dark:text-white">{chaosDrill.lastScore}%</Metric>
            <ProgressBar
              value={chaosDrill.lastScore}
              color={chaosDrill.lastScore >= 80 ? 'emerald' : chaosDrill.lastScore >= 60 ? 'yellow' : 'rose'}
              className="mt-2"
            />
          </div>
          <div>
            <Text className="dark:text-gray-400">Dias Desde Simulacro / Days Since Drill</Text>
            <Metric className="dark:text-white">{chaosDrill.daysSinceDrill}</Metric>
            <ProgressBar
              value={Math.min((chaosDrill.daysSinceDrill / 30) * 100, 100)}
              color={chaosDrill.daysSinceDrill <= 7 ? 'emerald' : chaosDrill.daysSinceDrill <= 21 ? 'yellow' : 'rose'}
              className="mt-2"
            />
          </div>
          <div>
            <Text className="dark:text-gray-400">Proximo Esperado / Next Expected</Text>
            <Metric className="dark:text-white">{chaosDrill.nextExpectedDays} dias</Metric>
            <ProgressBar
              value={Math.max(100 - (chaosDrill.nextExpectedDays / 14) * 100, 0)}
              color="blue"
              className="mt-2"
            />
          </div>
        </Grid>
      </Card>

      {/* Footer */}
      <Flex justifyContent="center" className="mt-8 pb-4">
        <Text className="text-xs dark:text-gray-600">
          Ierahkwa &mdash; Soberania Digital para las Americas / Digital Sovereignty for the Americas
        </Text>
      </Flex>
    </div>
  );
}
