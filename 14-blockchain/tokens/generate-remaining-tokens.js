#!/usr/bin/env node
/**
 * Genera las 108 naciones restantes para completar 574 SNT tokens
 * Continúa desde ID 0467 hasta 0574
 */
const fs = require('fs');
const path = require('path');

const REMAINING = [
  // More US Nations
  { name: "Sault Ste Marie Chippewa", symbol: "SAULTSTMARIE", region: "Great Lakes", country: "US", lang: "Ojibwe" },
  { name: "Keweenaw Bay Ojibwe", symbol: "KEWEENAW", region: "Great Lakes", country: "US", lang: "Ojibwe" },
  { name: "Lac Courte Oreilles", symbol: "LACCOURTE", region: "Great Lakes", country: "US", lang: "Ojibwe" },
  { name: "Sokaogon Chippewa (Mole Lake)", symbol: "MOLELAKE", region: "Great Lakes", country: "US", lang: "Ojibwe" },
  { name: "Bad River Ojibwe", symbol: "BADRIVER", region: "Great Lakes", country: "US", lang: "Ojibwe" },
  { name: "St Croix Chippewa", symbol: "STCROIX", region: "Great Lakes", country: "US", lang: "Ojibwe" },
  { name: "Forest County Potawatomi", symbol: "FORESTPOTAW", region: "Great Lakes", country: "US", lang: "Potawatomi" },
  { name: "Citizen Potawatomi", symbol: "CITIZENPOTAW", region: "Great Plains", country: "US", lang: "Potawatomi" },
  { name: "Prairie Band Potawatomi", symbol: "PRAIRIEPOTAW", region: "Great Plains", country: "US", lang: "Potawatomi" },
  { name: "Pokagon Potawatomi", symbol: "POKAGON", region: "Great Lakes", country: "US", lang: "Potawatomi" },
  { name: "Fort Belknap Assiniboine Gros Ventre", symbol: "FORTBELKNAP", region: "Great Plains", country: "US", lang: "Nakoda/Gros Ventre" },
  { name: "Fort Peck Assiniboine Sioux", symbol: "FORTPECK", region: "Great Plains", country: "US", lang: "Nakoda/Dakota" },
  { name: "Turtle Mountain Chippewa", symbol: "TURTLEMTN", region: "Great Plains", country: "US", lang: "Ojibwe/Michif" },
  { name: "Spirit Lake Nation", symbol: "SPIRITLAKE", region: "Great Plains", country: "US", lang: "Dakota" },
  { name: "Lower Brule Sioux", symbol: "LOWERBRULE", region: "Great Plains", country: "US", lang: "Lakota" },
  { name: "Crow Creek Sioux", symbol: "CROWCREEK", region: "Great Plains", country: "US", lang: "Lakota" },
  { name: "Three Affiliated Tribes (MHA Nation)", symbol: "MHANATION", region: "Great Plains", country: "US", lang: "Mandan/Hidatsa/Arikara" },
  { name: "Winnebago Tribe Nebraska", symbol: "WINNEBAGO", region: "Great Plains", country: "US", lang: "Ho-Chunk" },
  { name: "Santee Sioux", symbol: "SANTEESIOUX", region: "Great Plains", country: "US", lang: "Dakota" },
  { name: "Shakopee Mdewakanton", symbol: "SHAKOPEE", region: "Great Plains", country: "US", lang: "Dakota" },
  { name: "Upper Sioux", symbol: "UPPERSIOUX", region: "Great Plains", country: "US", lang: "Dakota" },
  { name: "Lower Sioux", symbol: "LOWERSIOUX", region: "Great Plains", country: "US", lang: "Dakota" },
  { name: "Prairie Island", symbol: "PRAIRIEISLAND", region: "Great Plains", country: "US", lang: "Dakota" },
  { name: "Fond du Lac Ojibwe", symbol: "FONDDULAC", region: "Great Lakes", country: "US", lang: "Ojibwe" },
  { name: "Bois Forte Ojibwe", symbol: "BOISFORTE", region: "Great Lakes", country: "US", lang: "Ojibwe" },
  { name: "Red Lake Ojibwe", symbol: "REDLAKE", region: "Great Lakes", country: "US", lang: "Ojibwe" },
  { name: "Grand Portage Ojibwe", symbol: "GRANDPORTAGE", region: "Great Lakes", country: "US", lang: "Ojibwe" },
  // More Southwest
  { name: "Salt River Pima-Maricopa", symbol: "SALTRIVER", region: "Southwest", country: "US", lang: "O'odham/Piipaash" },
  { name: "Gila River Indian Community", symbol: "GILARIVER", region: "Southwest", country: "US", lang: "O'odham/Piipaash" },
  { name: "Pascua Yaqui", symbol: "PASCUAYAQUI", region: "Southwest", country: "US", lang: "Yoeme" },
  { name: "Colorado River Indian Tribes", symbol: "COLORADORIVER", region: "Southwest", country: "US", lang: "Multiple" },
  { name: "Quechan (Fort Yuma)", symbol: "QUECHAN", region: "Southwest", country: "US", lang: "Quechan" },
  { name: "Ak-Chin Indian Community", symbol: "AKCHIN", region: "Southwest", country: "US", lang: "O'odham" },
  { name: "Camp Verde Yavapai-Apache", symbol: "CAMPVERDE", region: "Southwest", country: "US", lang: "Yavapai/Apache" },
  { name: "Tonto Apache Reservation", symbol: "TONTORES", region: "Southwest", country: "US", lang: "Western Apache" },
  { name: "Yavapai-Prescott", symbol: "YAVAPAIPRESCOTT", region: "Southwest", country: "US", lang: "Yavapai" },
  { name: "San Felipe Pueblo", symbol: "SANFELIPE", region: "Southwest", country: "US", lang: "Keresan" },
  { name: "Santa Ana Pueblo", symbol: "SANTAANA", region: "Southwest", country: "US", lang: "Keresan" },
  // More California
  { name: "San Manuel Band", symbol: "SANMANUEL", region: "California", country: "US", lang: "Serrano" },
  { name: "Soboba Band", symbol: "SOBOBA", region: "California", country: "US", lang: "Cahuilla/Luiseno" },
  { name: "Santa Ynez Chumash", symbol: "SANTAYNEZ", region: "California", country: "US", lang: "Chumash" },
  { name: "Tule River", symbol: "TULERIVER", region: "California", country: "US", lang: "Yokuts" },
  { name: "Bishop Paiute", symbol: "BISHOPPAIUTE", region: "California", country: "US", lang: "Paiute" },
  { name: "Big Pine Paiute", symbol: "BIGPINE", region: "California", country: "US", lang: "Paiute" },
  { name: "Fort Independence", symbol: "FORTINDEP", region: "California", country: "US", lang: "Paiute" },
  { name: "Table Mountain Rancheria", symbol: "TABLEMTN", region: "California", country: "US", lang: "Yokuts" },
  { name: "Tachi Yokut", symbol: "TACHIYOKUT", region: "California", country: "US", lang: "Yokuts" },
  { name: "Picayune Chukchansi", symbol: "CHUKCHANSI", region: "California", country: "US", lang: "Yokuts" },
  // More Pacific NW / Alaska
  { name: "Tulalip Tribes", symbol: "TULALIPTRIBES", region: "Pacific NW", country: "US", lang: "Lushootseed" },
  { name: "Puyallup Tribe", symbol: "PUYALLUP", region: "Pacific NW", country: "US", lang: "Lushootseed" },
  { name: "Nisqually", symbol: "NISQUALLY", region: "Pacific NW", country: "US", lang: "Lushootseed" },
  { name: "Squaxin Island", symbol: "SQUAXIN", region: "Pacific NW", country: "US", lang: "Lushootseed" },
  { name: "Skokomish", symbol: "SKOKOMISH", region: "Pacific NW", country: "US", lang: "Twana" },
  { name: "Suquamish", symbol: "SUQUAMISH", region: "Pacific NW", country: "US", lang: "Lushootseed" },
  { name: "Stillaguamish", symbol: "STILLAGUAMISH", region: "Pacific NW", country: "US", lang: "Lushootseed" },
  { name: "Swinomish", symbol: "SWINOMISH", region: "Pacific NW", country: "US", lang: "Lushootseed" },
  { name: "Upper Skagit", symbol: "UPPERSKAGIT", region: "Pacific NW", country: "US", lang: "Lushootseed" },
  { name: "Samish", symbol: "SAMISH", region: "Pacific NW", country: "US", lang: "Samish" },
  { name: "Nooksack", symbol: "NOOKSACK", region: "Pacific NW", country: "US", lang: "Nooksack" },
  { name: "Shoalwater Bay", symbol: "SHOALWATER", region: "Pacific NW", country: "US", lang: "Chehalis" },
  { name: "Chehalis", symbol: "CHEHALIS", region: "Pacific NW", country: "US", lang: "Chehalis" },
  { name: "Cowlitz", symbol: "COWLITZ", region: "Pacific NW", country: "US", lang: "Cowlitz" },
  { name: "Snohomish", symbol: "SNOHOMISH", region: "Pacific NW", country: "US", lang: "Lushootseed" },
  { name: "Kalispel", symbol: "KALISPEL", region: "Pacific NW", country: "US", lang: "Salish" },
  // Alaska Native
  { name: "Tlingit Central Council", symbol: "TLINGITCC", region: "Arctic", country: "US", lang: "Tlingit" },
  { name: "Metlakatla Indian Community", symbol: "METLAKATLA", region: "Arctic", country: "US", lang: "Tsimshian" },
  { name: "Central Council Tlingit Haida", symbol: "CCTLINGITHAIDA", region: "Arctic", country: "US", lang: "Tlingit/Haida" },
  { name: "Tanana Chiefs Conference", symbol: "TANANA", region: "Arctic", country: "US", lang: "Athabascan" },
  { name: "Association of Village Council Presidents", symbol: "AVCP", region: "Arctic", country: "US", lang: "Yup'ik" },
  { name: "Calista Corporation (Yup'ik)", symbol: "CALISTA", region: "Arctic", country: "US", lang: "Yup'ik" },
  { name: "Doyon (Interior Athabascan)", symbol: "DOYON", region: "Arctic", country: "US", lang: "Athabascan" },
  { name: "Sealaska (Southeast Alaska)", symbol: "SEALASKA", region: "Arctic", country: "US", lang: "Tlingit/Haida/Tsimshian" },
  { name: "NANA Regional Corporation", symbol: "NANA", region: "Arctic", country: "US", lang: "Inupiaq" },
  { name: "Arctic Slope Regional", symbol: "ARCTICSLOPE", region: "Arctic", country: "US", lang: "Inupiaq" },
  // More South America
  { name: "Kaingang", symbol: "KAINGANG", region: "Cono Sur", country: "BR", lang: "Kaingang" },
  { name: "Xokleng (Laklano)", symbol: "XOKLENG", region: "Cono Sur", country: "BR", lang: "Xokleng" },
  { name: "Maxakali (Tikmu'un)", symbol: "MAXAKALI", region: "Amazonia", country: "BR", lang: "Maxakali" },
  { name: "Rikbaktsa", symbol: "RIKBAKTSA", region: "Amazonia", country: "BR", lang: "Rikbaktsa" },
  { name: "Nambikwara", symbol: "NAMBIKWARA", region: "Amazonia", country: "BR", lang: "Nambikwara" },
  { name: "Suruí (Paiter)", symbol: "SURUI", region: "Amazonia", country: "BR", lang: "Suruí" },
  { name: "Gavião", symbol: "GAVIAO", region: "Amazonia", country: "BR", lang: "Gavião" },
  { name: "Cinta Larga", symbol: "CINTALARGA", region: "Amazonia", country: "BR", lang: "Cinta Larga" },
  { name: "Wajãpi", symbol: "WAJAPI", region: "Amazonia", country: "BR/GF", lang: "Wajãpi" },
  { name: "Apinajé", symbol: "APINAJE", region: "Amazonia", country: "BR", lang: "Apinajé" },
  { name: "Canela (Ramkokamekrá)", symbol: "CANELA", region: "Amazonia", country: "BR", lang: "Canela" },
  { name: "Xerente", symbol: "XERENTE", region: "Amazonia", country: "BR", lang: "Xerente" },
  { name: "Karajá", symbol: "KARAJA", region: "Amazonia", country: "BR", lang: "Karajá" },
  { name: "Tapirapé", symbol: "TAPIRAPE", region: "Amazonia", country: "BR", lang: "Tapirapé" },
  { name: "Parakanã", symbol: "PARAKANA", region: "Amazonia", country: "BR", lang: "Parakanã" },
  { name: "Araweté", symbol: "ARAWETE", region: "Amazonia", country: "BR", lang: "Araweté" },
  { name: "Asurini do Xingu", symbol: "ASURINI", region: "Amazonia", country: "BR", lang: "Asurini" },
  { name: "Mebêngôkre Xikrin", symbol: "XIKRIN", region: "Amazonia", country: "BR", lang: "Mẽbêngôkre" },
  { name: "Baré", symbol: "BARE", region: "Amazonia", country: "BR", lang: "Baré" },
  { name: "Piratapuya", symbol: "PIRATAPUYA", region: "Amazonia", country: "BR/CO", lang: "Piratapuya" },
  { name: "Tariana", symbol: "TARIANA", region: "Amazonia", country: "BR", lang: "Tariana" },
  { name: "Ye'kwana (Maquiritare)", symbol: "YEKWANA", region: "Amazonia", country: "VE/BR", lang: "Ye'kwana" },
  { name: "Piaroa (Huottüja)", symbol: "PIAROA", region: "Amazonia", country: "VE", lang: "Piaroa" },
  { name: "Yanomami de Venezuela", symbol: "YANOMAMIVE", region: "Amazonia", country: "VE", lang: "Yanomam" },
  { name: "Warekena", symbol: "WAREKENA", region: "Amazonia", country: "VE/BR", lang: "Warekena" },
  { name: "Panare (E'ñapa)", symbol: "PANARE", region: "Amazonia", country: "VE", lang: "E'ñapa" },
  { name: "Jivi (Guahibo)", symbol: "JIVI", region: "Amazonia", country: "VE/CO", lang: "Jivi" },
  { name: "Puinave", symbol: "PUINAVE", region: "Amazonia", country: "CO/VE", lang: "Puinave" },
  { name: "Sikuani (Guahibo CO)", symbol: "SIKUANI", region: "Amazonia", country: "CO", lang: "Sikuani" },
];

const baseDir = path.join(__dirname, 'nations');
const manifestPath = path.join(baseDir, 'MANIFEST.json');
const existing = JSON.parse(fs.readFileSync(manifestPath, 'utf8'));
let startId = existing.tokens.length;

REMAINING.forEach((nation, index) => {
  startId++;
  const id = String(startId).padStart(4, '0');
  const symbol = `SNT-${nation.symbol}`;
  const contractAddr = `0x574${String(startId).padStart(4, '0')}${'0'.repeat(36)}`;

  const token = {
    id, name: nation.name, symbol,
    standard: "SNT (Sovereign Nation Token)",
    description: `Token soberano de la nacion ${nation.name}. Representa la soberania digital, derechos de gobernanza y participacion economica en la plataforma Ierahkwa Ne Kanienke.`,
    region: nation.region, country: nation.country, language: nation.lang,
    image: `https://ierahkwa.gov/tokens/nations/${nation.symbol.toLowerCase()}.png`,
    external_url: `https://ierahkwa.gov/nations/${nation.symbol.toLowerCase()}`,
    attributes: [
      { trait_type: "Nation ID", value: id },
      { trait_type: "Region", value: nation.region },
      { trait_type: "Country", value: nation.country },
      { trait_type: "Language", value: nation.lang },
      { trait_type: "Token Standard", value: "SNT (Sovereign Nation Token)" },
      { trait_type: "Blockchain", value: "MameyNode" },
      { trait_type: "Chain ID", value: "574" },
      { trait_type: "Status", value: "Pre-minted — Awaiting Sovereign Recognition" }
    ],
    blockchain: {
      network: "MameyNode", chainId: 574,
      consensus: "Sovereign Proof of Stake (SPoS)",
      contract: contractAddr, standard: "SNT-574",
      centralBank: "BDET Bank", nativeCurrency: "WAMPUM (WPM)"
    },
    decimals: 18, totalSupply: "1000000000",
    distribution: { tribalCouncil: "40%", citizenAirdrop: "25%", developmentFund: "15%", educationFund: "10%", culturalPreservation: "5%", liquidityPool: "5%" },
    governance: { type: "Tribal Council DAO", votingPeriod: "7 days", quorum: "10%", tribalVerification: true, councilSeats: 21 },
    status: "pre-minted",
    activationCondition: "Sovereign recognition signature by tribal council"
  };

  fs.writeFileSync(path.join(baseDir, `${id}-${symbol}.json`), JSON.stringify(token, null, 2));
  existing.tokens.push({ id, symbol, name: nation.name, region: nation.region, country: nation.country, language: nation.lang, contract: contractAddr, status: "pre-minted" });
});

existing.totalNations = existing.tokens.length;
existing.generatedAt = new Date().toISOString();
fs.writeFileSync(manifestPath, JSON.stringify(existing, null, 2));

console.log(`Added ${REMAINING.length} more tokens. Total: ${existing.totalNations}`);
