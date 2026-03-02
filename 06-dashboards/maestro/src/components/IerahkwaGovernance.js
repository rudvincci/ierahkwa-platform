/**
 * IerahkwaGovernance.js
 * Interfaz de Gobernanza DAO / DAO Governance Interface
 * Plataforma Ierahkwa - Soberania Digital para las Americas
 */

import React, { useState, useEffect, useCallback, useMemo } from 'react';
import { ethers } from 'ethers';
import {
  Card,
  Title,
  Text,
  Metric,
  DonutChart,
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

const PROPOSAL_TYPES = [
  { value: 'humanitarian', label: 'Humanitario / Humanitarian', color: 'emerald' },
  { value: 'environmental', label: 'Ambiental / Environmental', color: 'teal' },
  { value: 'infrastructure', label: 'Infraestructura / Infrastructure', color: 'blue' },
  { value: 'education', label: 'Educacion / Education', color: 'violet' },
  { value: 'emergency', label: 'Emergencia / Emergency', color: 'rose' },
];

const MATTR_TIERS = [
  { name: 'Observador', min: 0, max: 99, color: 'slate', minRepToPropose: false },
  { name: 'Guardian', min: 100, max: 499, color: 'blue', minRepToPropose: true },
  { name: 'Anciano / Elder', min: 500, max: 999, color: 'amber', minRepToPropose: true },
  { name: 'Consejo / Council', min: 1000, max: Infinity, color: 'emerald', minRepToPropose: true },
];

const REFRESH_INTERVAL_MS = 30000;

function getMattrTier(score) {
  return MATTR_TIERS.find((t) => score >= t.min && score <= t.max) || MATTR_TIERS[0];
}

function quadraticWeight(votes) {
  return Math.sqrt(votes);
}

function formatCountdown(endTimestamp) {
  const now = Date.now();
  const diff = endTimestamp - now;
  if (diff <= 0) return 'Expirado / Expired';
  const hours = Math.floor(diff / 3_600_000);
  const minutes = Math.floor((diff % 3_600_000) / 60_000);
  if (hours > 24) {
    const days = Math.floor(hours / 24);
    return `${days}d ${hours % 24}h`;
  }
  return `${hours}h ${minutes}m`;
}

function truncateAddress(addr) {
  if (!addr) return '';
  return `${addr.slice(0, 6)}...${addr.slice(-4)}`;
}

// ============================================================
// Mock API / Simulated Data Fetchers
// ============================================================

async function fetchActiveProposals() {
  try {
    const res = await fetch('/api/governance/proposals/active');
    if (res.ok) return res.json();
  } catch (_) { /* fallback */ }
  const types = PROPOSAL_TYPES;
  return Array.from({ length: 5 }, (_, i) => {
    const type = types[i % types.length];
    const votesFor = Math.floor(Math.random() * 5000);
    const votesAgainst = Math.floor(Math.random() * 3000);
    const endTime = Date.now() + Math.floor(Math.random() * 7 * 86_400_000);
    return {
      id: `PROP-${String(1000 + i)}`,
      title: [
        'Fondo de Emergencia Climatica / Climate Emergency Fund',
        'Red Educativa Descentralizada / Decentralized Education Network',
        'Expansion de Nodos LoRa / LoRa Node Expansion',
        'Programa Salud Comunitaria / Community Health Program',
        'Alerta Rapida Sísmica / Seismic Early Warning',
      ][i],
      description: [
        'Asignar 500,000 WMP para respuesta a desastres climaticos en el Caribe.',
        'Crear red de contenido educativo distribuido por IPFS para comunidades rurales.',
        'Instalar 200 nodos LoRa adicionales en Centro America para cobertura completa.',
        'Financiar clinicas moviles en regiones desatendidas de Sur America.',
        'Desplegar sensores sismicos conectados a la red mesh para alerta temprana.',
      ][i],
      type: type.value,
      typeLabel: type.label,
      typeColor: type.color,
      wmpRequested: (i + 1) * 100_000,
      votesFor,
      votesAgainst,
      quadraticFor: quadraticWeight(votesFor),
      quadraticAgainst: quadraticWeight(votesAgainst),
      endTimestamp: endTime,
      ipfsHash: `Qm${Math.random().toString(36).substring(2, 14)}`,
      proposer: `0x${Math.random().toString(16).substring(2, 14)}`,
      minReputation: 100,
    };
  });
}

async function fetchProposalHistory() {
  try {
    const res = await fetch('/api/governance/proposals/history');
    if (res.ok) return res.json();
  } catch (_) { /* fallback */ }
  const statuses = ['Aprobado / Passed', 'Rechazado / Rejected', 'Expirado / Expired'];
  const statusColors = { 'Aprobado / Passed': 'emerald', 'Rechazado / Rejected': 'rose', 'Expirado / Expired': 'gray' };
  return Array.from({ length: 10 }, (_, i) => {
    const status = statuses[Math.floor(Math.random() * statuses.length)];
    return {
      id: `PROP-${String(900 + i)}`,
      title: `Propuesta Historica ${900 + i}`,
      type: PROPOSAL_TYPES[i % PROPOSAL_TYPES.length].label,
      status,
      statusColor: statusColors[status],
      votesFor: Math.floor(Math.random() * 8000),
      votesAgainst: Math.floor(Math.random() * 5000),
      wmpAmount: Math.floor(Math.random() * 500_000),
      closedDate: new Date(Date.now() - Math.random() * 90 * 86_400_000).toISOString(),
    };
  });
}

async function fetchTreasuryAllocation() {
  try {
    const res = await fetch('/api/governance/treasury/allocation');
    if (res.ok) return res.json();
  } catch (_) { /* fallback */ }
  return [
    { category: 'Humanitario / Humanitarian', amount: 5_200_000, pct: 35.2 },
    { category: 'Ambiental / Environmental', amount: 2_900_000, pct: 19.7 },
    { category: 'Infraestructura / Infrastructure', amount: 2_400_000, pct: 16.3 },
    { category: 'Educacion / Education', amount: 2_050_000, pct: 13.9 },
    { category: 'Reserva Emergencia / Emergency Reserve', amount: 2_200_000, pct: 14.9 },
  ];
}

async function fetchWalletReputation(address) {
  try {
    const res = await fetch(`/api/governance/reputation/${address}`);
    if (res.ok) return res.json();
  } catch (_) { /* fallback */ }
  const score = Math.floor(Math.random() * 1200);
  return {
    address,
    mattrScore: score,
    tier: getMattrTier(score),
    votingPower: quadraticWeight(score).toFixed(2),
    proposalsCreated: Math.floor(Math.random() * 12),
    proposalsVoted: Math.floor(Math.random() * 50),
  };
}

// ============================================================
// Sub-Components
// ============================================================

/** Tarjeta de Propuesta / Proposal Card */
function ProposalCard({ proposal, onVote, walletConnected }) {
  const totalQuadratic = proposal.quadraticFor + proposal.quadraticAgainst;
  const forPct = totalQuadratic > 0 ? (proposal.quadraticFor / totalQuadratic) * 100 : 50;
  const againstPct = totalQuadratic > 0 ? (proposal.quadraticAgainst / totalQuadratic) * 100 : 50;
  const isEmergency = proposal.type === 'emergency';
  const timeLeft = formatCountdown(proposal.endTimestamp);

  return (
    <Card
      className={`dark:bg-gray-900 dark:ring-gray-800 ${isEmergency ? 'ring-2 ring-rose-500' : ''}`}
      decoration="left"
      decorationColor={proposal.typeColor}
    >
      <Flex justifyContent="between" alignItems="start">
        <div className="flex-1">
          <Flex justifyContent="start" alignItems="center" className="gap-2 mb-1">
            <Text className="font-mono text-xs dark:text-gray-500">{proposal.id}</Text>
            <Badge color={proposal.typeColor} size="sm">{proposal.typeLabel}</Badge>
            {isEmergency && <Badge color="rose" size="sm">24h</Badge>}
          </Flex>
          <Title className="text-lg dark:text-white">{proposal.title}</Title>
          <Text className="mt-1 dark:text-gray-400">{proposal.description}</Text>
        </div>
        <div className="text-right ml-4 shrink-0">
          <Text className="dark:text-gray-400">Tiempo / Time</Text>
          <Metric className="text-lg dark:text-white">{timeLeft}</Metric>
        </div>
      </Flex>

      <div className="mt-4 space-y-2">
        <Flex justifyContent="between">
          <Text className="dark:text-gray-400">
            A Favor / For: {proposal.votesFor.toLocaleString()} votos ({forPct.toFixed(1)}% cuadratico)
          </Text>
          <Text className="dark:text-gray-400">
            En Contra / Against: {proposal.votesAgainst.toLocaleString()} votos ({againstPct.toFixed(1)}%)
          </Text>
        </Flex>
        <ProgressBar value={forPct} color="emerald" className="h-2" />
      </div>

      <Flex justifyContent="between" alignItems="center" className="mt-4">
        <div>
          <Text className="text-xs dark:text-gray-500">
            Solicitado / Requested: {proposal.wmpRequested.toLocaleString()} WMP
          </Text>
          <Text className="text-xs dark:text-gray-500">
            IPFS: {proposal.ipfsHash}
          </Text>
        </div>
        <div className="flex gap-2">
          <button
            onClick={() => onVote(proposal.id, 'for')}
            disabled={!walletConnected}
            className="px-4 py-2 rounded-lg bg-emerald-600 text-white text-sm font-medium
                       hover:bg-emerald-700 disabled:opacity-40 disabled:cursor-not-allowed
                       transition-colors"
          >
            Votar A Favor / Vote For
          </button>
          <button
            onClick={() => onVote(proposal.id, 'against')}
            disabled={!walletConnected}
            className="px-4 py-2 rounded-lg bg-rose-600 text-white text-sm font-medium
                       hover:bg-rose-700 disabled:opacity-40 disabled:cursor-not-allowed
                       transition-colors"
          >
            Votar En Contra / Vote Against
          </button>
        </div>
      </Flex>
    </Card>
  );
}

/** Modal para Crear Propuesta / Create Proposal Modal */
function CreateProposalModal({ isOpen, onClose, onSubmit, walletRep }) {
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [proposalType, setProposalType] = useState('humanitarian');
  const [wmpAmount, setWmpAmount] = useState('');
  const [ipfsHash, setIpfsHash] = useState('');

  if (!isOpen) return null;

  const canPropose = walletRep && walletRep.mattrScore >= 100;

  const handleSubmit = (e) => {
    e.preventDefault();
    onSubmit({ title, description, type: proposalType, wmpAmount: Number(wmpAmount), ipfsHash });
    setTitle('');
    setDescription('');
    setProposalType('humanitarian');
    setWmpAmount('');
    setIpfsHash('');
    onClose();
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/60 p-4">
      <Card className="w-full max-w-2xl max-h-[90vh] overflow-y-auto dark:bg-gray-900 dark:ring-gray-700">
        <Flex justifyContent="between" alignItems="center" className="mb-4">
          <Title className="dark:text-white">
            Crear Propuesta / Create Proposal
          </Title>
          <button
            onClick={onClose}
            className="text-gray-500 hover:text-gray-700 dark:hover:text-gray-300 text-2xl leading-none"
          >
            &times;
          </button>
        </Flex>

        {!canPropose && (
          <Card className="mb-4 bg-amber-50 dark:bg-amber-900/20 ring-amber-300">
            <Text className="text-amber-800 dark:text-amber-300">
              Se requiere nivel Guardian (100+ $MATTR) para crear propuestas.
              Guardian level (100+ $MATTR) required to create proposals.
            </Text>
            <Text className="text-sm text-amber-700 dark:text-amber-400 mt-1">
              Tu puntaje actual / Your current score: {walletRep?.mattrScore ?? 0} $MATTR
            </Text>
          </Card>
        )}

        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium dark:text-gray-300 mb-1">
              Titulo / Title
            </label>
            <input
              type="text"
              value={title}
              onChange={(e) => setTitle(e.target.value)}
              required
              className="w-full px-3 py-2 border rounded-lg dark:bg-gray-800 dark:border-gray-700
                         dark:text-white focus:ring-2 focus:ring-emerald-500 focus:border-emerald-500"
              placeholder="Titulo de la propuesta..."
            />
          </div>

          <div>
            <label className="block text-sm font-medium dark:text-gray-300 mb-1">
              Descripcion / Description (Markdown)
            </label>
            <textarea
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              required
              rows={5}
              className="w-full px-3 py-2 border rounded-lg dark:bg-gray-800 dark:border-gray-700
                         dark:text-white focus:ring-2 focus:ring-emerald-500 focus:border-emerald-500
                         font-mono text-sm"
              placeholder="Descripcion detallada con soporte Markdown..."
            />
          </div>

          <div>
            <label className="block text-sm font-medium dark:text-gray-300 mb-1">
              Tipo de Propuesta / Proposal Type
            </label>
            <select
              value={proposalType}
              onChange={(e) => setProposalType(e.target.value)}
              className="w-full px-3 py-2 border rounded-lg dark:bg-gray-800 dark:border-gray-700
                         dark:text-white focus:ring-2 focus:ring-emerald-500 focus:border-emerald-500"
            >
              {PROPOSAL_TYPES.map((pt) => (
                <option key={pt.value} value={pt.value}>{pt.label}</option>
              ))}
            </select>
          </div>

          <div>
            <label className="block text-sm font-medium dark:text-gray-300 mb-1">
              Cantidad WMP Solicitada / WMP Amount Requested
            </label>
            <input
              type="number"
              value={wmpAmount}
              onChange={(e) => setWmpAmount(e.target.value)}
              required
              min="1"
              className="w-full px-3 py-2 border rounded-lg dark:bg-gray-800 dark:border-gray-700
                         dark:text-white focus:ring-2 focus:ring-emerald-500 focus:border-emerald-500"
              placeholder="100000"
            />
          </div>

          <div>
            <label className="block text-sm font-medium dark:text-gray-300 mb-1">
              IPFS Metadata Hash
            </label>
            <input
              type="text"
              value={ipfsHash}
              onChange={(e) => setIpfsHash(e.target.value)}
              className="w-full px-3 py-2 border rounded-lg dark:bg-gray-800 dark:border-gray-700
                         dark:text-white focus:ring-2 focus:ring-emerald-500 focus:border-emerald-500
                         font-mono text-sm"
              placeholder="QmYwAPJzv5CZsnA625s3Xf2nemtYg..."
            />
          </div>

          <div className="bg-gray-100 dark:bg-gray-800 rounded-lg p-3">
            <Text className="text-sm dark:text-gray-400">
              Requisito de Reputacion / Reputation Requirement: 100+ $MATTR (Guardian)
            </Text>
            <Text className="text-sm dark:text-gray-400">
              Formula de Voto / Voting Formula: Poder = sqrt(votos) &mdash; Voto cuadratico / Quadratic voting
            </Text>
          </div>

          <Flex justifyContent="end" className="gap-3 pt-2">
            <button
              type="button"
              onClick={onClose}
              className="px-4 py-2 rounded-lg border dark:border-gray-600 dark:text-gray-300
                         hover:bg-gray-100 dark:hover:bg-gray-800 transition-colors"
            >
              Cancelar / Cancel
            </button>
            <button
              type="submit"
              disabled={!canPropose}
              className="px-6 py-2 rounded-lg bg-emerald-600 text-white font-medium
                         hover:bg-emerald-700 disabled:opacity-40 disabled:cursor-not-allowed
                         transition-colors"
            >
              Enviar Propuesta / Submit Proposal
            </button>
          </Flex>
        </form>
      </Card>
    </div>
  );
}

// ============================================================
// Componente Principal / Main Component
// ============================================================

export default function IerahkwaGovernance() {
  // ----- Wallet State -----
  const [walletAddress, setWalletAddress] = useState(null);
  const [walletRep, setWalletRep] = useState(null);
  const [connectError, setConnectError] = useState(null);

  // ----- Data State -----
  const [activeProposals, setActiveProposals] = useState([]);
  const [proposalHistory, setProposalHistory] = useState([]);
  const [treasuryAllocation, setTreasuryAllocation] = useState([]);
  const [historySortField, setHistorySortField] = useState('closedDate');
  const [historySortDir, setHistorySortDir] = useState('desc');
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [loading, setLoading] = useState(true);

  // ----- Wallet Connection -----
  const connectWallet = useCallback(async () => {
    setConnectError(null);
    try {
      if (typeof window === 'undefined' || !window.ethereum) {
        setConnectError('MetaMask no detectado. Instale MetaMask para continuar. / MetaMask not detected.');
        return;
      }
      const provider = new ethers.BrowserProvider(window.ethereum);
      const accounts = await provider.send('eth_requestAccounts', []);
      if (accounts.length > 0) {
        setWalletAddress(accounts[0]);
        const rep = await fetchWalletReputation(accounts[0]);
        setWalletRep(rep);
      }
    } catch (err) {
      setConnectError(`Error de conexion: ${err.message}`);
    }
  }, []);

  const disconnectWallet = useCallback(() => {
    setWalletAddress(null);
    setWalletRep(null);
    setConnectError(null);
  }, []);

  // ----- Data Fetching -----
  const refreshData = useCallback(async () => {
    setLoading(true);
    const [proposals, history, allocation] = await Promise.all([
      fetchActiveProposals(),
      fetchProposalHistory(),
      fetchTreasuryAllocation(),
    ]);
    setActiveProposals(proposals);
    setProposalHistory(history);
    setTreasuryAllocation(allocation);
    setLoading(false);
  }, []);

  useEffect(() => {
    refreshData();
    const interval = setInterval(refreshData, REFRESH_INTERVAL_MS);
    return () => clearInterval(interval);
  }, [refreshData]);

  // ----- Handlers -----
  const handleVote = useCallback(async (proposalId, direction) => {
    if (!walletAddress) return;
    // In production this would call the smart contract
    console.log(`[Ierahkwa] Voto / Vote: ${direction} en / on ${proposalId} desde / from ${walletAddress}`);
    alert(`Voto registrado: ${direction === 'for' ? 'A Favor' : 'En Contra'} en ${proposalId}\nVote recorded: ${direction} on ${proposalId}`);
    await refreshData();
  }, [walletAddress, refreshData]);

  const handleCreateProposal = useCallback(async (data) => {
    console.log('[Ierahkwa] Nueva propuesta / New proposal:', data);
    alert(`Propuesta creada: ${data.title}\nProposal created: ${data.title}`);
    await refreshData();
  }, [refreshData]);

  // ----- Sorted History -----
  const sortedHistory = useMemo(() => {
    const sorted = [...proposalHistory];
    sorted.sort((a, b) => {
      let valA = a[historySortField];
      let valB = b[historySortField];
      if (typeof valA === 'string') valA = valA.toLowerCase();
      if (typeof valB === 'string') valB = valB.toLowerCase();
      if (valA < valB) return historySortDir === 'asc' ? -1 : 1;
      if (valA > valB) return historySortDir === 'asc' ? 1 : -1;
      return 0;
    });
    return sorted;
  }, [proposalHistory, historySortField, historySortDir]);

  const toggleSort = (field) => {
    if (historySortField === field) {
      setHistorySortDir((d) => (d === 'asc' ? 'desc' : 'asc'));
    } else {
      setHistorySortField(field);
      setHistorySortDir('desc');
    }
  };

  // ----- Derived -----
  const emergencyProposals = activeProposals.filter((p) => p.type === 'emergency');
  const standardProposals = activeProposals.filter((p) => p.type !== 'emergency');
  const treasuryColors = ['emerald', 'teal', 'blue', 'violet', 'rose'];

  // ----- Render -----
  return (
    <div className="ierahkwa-governance p-4 md:p-6 lg:p-8 min-h-screen bg-gray-50 dark:bg-gray-950">
      {/* Header */}
      <Flex justifyContent="between" alignItems="center" className="mb-6 flex-wrap gap-4">
        <div>
          <Title className="text-2xl font-bold dark:text-white">
            Gobernanza Ierahkwa / Ierahkwa Governance
          </Title>
          <Text className="dark:text-gray-400">
            DAO de Soberania para las Americas / Sovereignty DAO for the Americas
          </Text>
        </div>
        <div className="flex items-center gap-3">
          {walletAddress ? (
            <Flex alignItems="center" className="gap-3">
              <Badge color="emerald" size="lg">
                {truncateAddress(walletAddress)}
              </Badge>
              {walletRep && (
                <Badge color={walletRep.tier.color} size="sm">
                  {walletRep.tier.name}: {walletRep.mattrScore} $MATTR
                </Badge>
              )}
              <button
                onClick={disconnectWallet}
                className="px-3 py-1.5 rounded-lg border dark:border-gray-600 text-sm
                           dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-800 transition-colors"
              >
                Desconectar / Disconnect
              </button>
            </Flex>
          ) : (
            <button
              onClick={connectWallet}
              className="px-4 py-2 rounded-lg bg-emerald-600 text-white font-medium
                         hover:bg-emerald-700 transition-colors flex items-center gap-2"
            >
              <span>Conectar Billetera / Connect Wallet</span>
            </button>
          )}
        </div>
      </Flex>

      {connectError && (
        <Card className="mb-4 bg-rose-50 dark:bg-rose-900/20 ring-rose-300">
          <Text className="text-rose-700 dark:text-rose-300">{connectError}</Text>
        </Card>
      )}

      {/* Emergency Proposals */}
      {emergencyProposals.length > 0 && (
        <div className="mb-6">
          <Flex justifyContent="start" alignItems="center" className="gap-2 mb-3">
            <Title className="dark:text-white">
              Propuestas de Emergencia / Emergency Proposals
            </Title>
            <Badge color="rose" size="lg">24h</Badge>
          </Flex>
          <div className="space-y-4">
            {emergencyProposals.map((proposal) => (
              <ProposalCard
                key={proposal.id}
                proposal={proposal}
                onVote={handleVote}
                walletConnected={!!walletAddress}
              />
            ))}
          </div>
        </div>
      )}

      {/* Main Grid: Proposals + Sidebar */}
      <Grid numItems={1} numItemsLg={3} className="gap-6 mb-6">
        {/* Active Proposals (2/3 width) */}
        <Col numColSpan={1} numColSpanLg={2}>
          <Flex justifyContent="between" alignItems="center" className="mb-3">
            <Title className="dark:text-white">
              Propuestas Activas / Active Proposals
            </Title>
            <button
              onClick={() => setShowCreateModal(true)}
              disabled={!walletAddress}
              className="px-4 py-2 rounded-lg bg-blue-600 text-white text-sm font-medium
                         hover:bg-blue-700 disabled:opacity-40 disabled:cursor-not-allowed
                         transition-colors"
            >
              + Nueva Propuesta / New Proposal
            </button>
          </Flex>
          <div className="space-y-4">
            {standardProposals.map((proposal) => (
              <ProposalCard
                key={proposal.id}
                proposal={proposal}
                onVote={handleVote}
                walletConnected={!!walletAddress}
              />
            ))}
            {standardProposals.length === 0 && (
              <Card className="dark:bg-gray-900">
                <Text className="text-center dark:text-gray-400">
                  No hay propuestas activas. / No active proposals.
                </Text>
              </Card>
            )}
          </div>
        </Col>

        {/* Sidebar (1/3 width) */}
        <Col>
          {/* Guardian Reputation Panel */}
          <Card className="mb-6 dark:bg-gray-900 dark:ring-gray-800">
            <Title className="dark:text-white">
              Panel de Reputacion / Reputation Panel
            </Title>
            {walletRep ? (
              <div className="mt-4 space-y-4">
                <div>
                  <Text className="dark:text-gray-400">Nivel / Tier</Text>
                  <Flex justifyContent="start" alignItems="center" className="gap-2 mt-1">
                    <Badge color={walletRep.tier.color} size="lg">{walletRep.tier.name}</Badge>
                    <Metric className="text-lg dark:text-white">{walletRep.mattrScore} $MATTR</Metric>
                  </Flex>
                </div>
                <div>
                  <Text className="dark:text-gray-400">Poder de Voto / Voting Power</Text>
                  <Metric className="dark:text-white">{walletRep.votingPower}</Metric>
                  <Text className="text-xs dark:text-gray-500 mt-1">
                    Formula: sqrt(puntaje) / Formula: sqrt(score)
                  </Text>
                </div>
                <Grid numItems={2} className="gap-3">
                  <Card className="dark:bg-gray-800">
                    <Text className="text-xs dark:text-gray-400">Propuestas Creadas / Created</Text>
                    <Metric className="text-lg dark:text-white">{walletRep.proposalsCreated}</Metric>
                  </Card>
                  <Card className="dark:bg-gray-800">
                    <Text className="text-xs dark:text-gray-400">Votos Emitidos / Votes Cast</Text>
                    <Metric className="text-lg dark:text-white">{walletRep.proposalsVoted}</Metric>
                  </Card>
                </Grid>
                <div>
                  <Text className="text-xs dark:text-gray-500">
                    Progreso al siguiente nivel / Progress to next tier
                  </Text>
                  <ProgressBar
                    value={(() => {
                      const tier = walletRep.tier;
                      if (tier.max === Infinity) return 100;
                      return ((walletRep.mattrScore - tier.min) / (tier.max - tier.min)) * 100;
                    })()}
                    color={walletRep.tier.color}
                    className="mt-1"
                  />
                </div>
              </div>
            ) : (
              <Text className="mt-4 dark:text-gray-400">
                Conecte su billetera para ver su reputacion. /
                Connect your wallet to view reputation.
              </Text>
            )}
          </Card>

          {/* Treasury Overview */}
          <Card className="dark:bg-gray-900 dark:ring-gray-800">
            <Title className="dark:text-white">
              Tesoreria / Treasury Overview
            </Title>
            <DonutChart
              data={treasuryAllocation}
              category="amount"
              index="category"
              colors={treasuryColors}
              className="h-48 mt-4"
              valueFormatter={(v) => `${(v / 1_000_000).toFixed(1)}M WMP`}
              showAnimation
            />
            <div className="mt-3 space-y-1">
              {treasuryAllocation.map((item, idx) => (
                <Flex key={item.category} justifyContent="between" alignItems="center">
                  <Flex justifyContent="start" alignItems="center" className="gap-2">
                    <Badge color={treasuryColors[idx]} size="xs">&nbsp;</Badge>
                    <Text className="text-xs dark:text-gray-400">{item.category}</Text>
                  </Flex>
                  <Text className="text-xs font-medium dark:text-gray-300">
                    {item.pct}%
                  </Text>
                </Flex>
              ))}
            </div>
          </Card>
        </Col>
      </Grid>

      {/* Proposal History */}
      <Card className="mb-6 dark:bg-gray-900 dark:ring-gray-800">
        <Title className="dark:text-white">
          Historial de Propuestas / Proposal History
        </Title>
        <Text className="mb-4 dark:text-gray-400">
          Haga clic en encabezados para ordenar / Click headers to sort
        </Text>
        <Table>
          <TableHead>
            <TableRow>
              <TableHeaderCell
                className="cursor-pointer dark:text-gray-300 hover:text-emerald-600"
                onClick={() => toggleSort('id')}
              >
                ID {historySortField === 'id' ? (historySortDir === 'asc' ? '\u2191' : '\u2193') : ''}
              </TableHeaderCell>
              <TableHeaderCell
                className="cursor-pointer dark:text-gray-300 hover:text-emerald-600"
                onClick={() => toggleSort('title')}
              >
                Titulo / Title {historySortField === 'title' ? (historySortDir === 'asc' ? '\u2191' : '\u2193') : ''}
              </TableHeaderCell>
              <TableHeaderCell className="dark:text-gray-300">Tipo / Type</TableHeaderCell>
              <TableHeaderCell
                className="cursor-pointer dark:text-gray-300 hover:text-emerald-600"
                onClick={() => toggleSort('status')}
              >
                Estado / Status {historySortField === 'status' ? (historySortDir === 'asc' ? '\u2191' : '\u2193') : ''}
              </TableHeaderCell>
              <TableHeaderCell
                className="cursor-pointer dark:text-gray-300 hover:text-emerald-600"
                onClick={() => toggleSort('votesFor')}
              >
                Votos A Favor / For {historySortField === 'votesFor' ? (historySortDir === 'asc' ? '\u2191' : '\u2193') : ''}
              </TableHeaderCell>
              <TableHeaderCell className="dark:text-gray-300">WMP</TableHeaderCell>
              <TableHeaderCell
                className="cursor-pointer dark:text-gray-300 hover:text-emerald-600"
                onClick={() => toggleSort('closedDate')}
              >
                Fecha / Date {historySortField === 'closedDate' ? (historySortDir === 'asc' ? '\u2191' : '\u2193') : ''}
              </TableHeaderCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {sortedHistory.map((item) => (
              <TableRow key={item.id}>
                <TableCell className="font-mono text-sm dark:text-gray-300">{item.id}</TableCell>
                <TableCell className="dark:text-gray-300">{item.title}</TableCell>
                <TableCell className="dark:text-gray-400 text-sm">{item.type}</TableCell>
                <TableCell>
                  <Badge color={item.statusColor} size="sm">{item.status}</Badge>
                </TableCell>
                <TableCell className="dark:text-gray-300">
                  {item.votesFor.toLocaleString()} / {item.votesAgainst.toLocaleString()}
                </TableCell>
                <TableCell className="dark:text-gray-300">
                  {item.wmpAmount.toLocaleString()}
                </TableCell>
                <TableCell className="dark:text-gray-400 text-sm">
                  {new Date(item.closedDate).toLocaleDateString('es-419')}
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </Card>

      {/* Create Proposal Modal */}
      <CreateProposalModal
        isOpen={showCreateModal}
        onClose={() => setShowCreateModal(false)}
        onSubmit={handleCreateProposal}
        walletRep={walletRep}
      />

      {/* Footer */}
      <Flex justifyContent="center" className="mt-8 pb-4">
        <Text className="text-xs dark:text-gray-600">
          Ierahkwa &mdash; Gobernanza Soberana para 1B+ personas en las Americas /
          Sovereign Governance for 1B+ people across the Americas
        </Text>
      </Flex>
    </div>
  );
}
