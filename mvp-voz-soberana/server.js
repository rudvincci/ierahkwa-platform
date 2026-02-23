const express = require('express');
const cors = require('cors');
const path = require('path');
const app = express();
const PORT = 3002;
app.use(cors());
app.use(express.json());
app.use(express.static(path.join(__dirname, 'public')));
let nextVozId = 11;
const ciudadanos = [
  {id:1,nombre:'Ixchel Tlanextli',handle:'@ixchel_soberana',bio:'Defensora de soberania digital indigena.',verificado:true,avatar_initials:'IT',seguidores:1284,siguiendo:342,voces_count:87},
  {id:2,nombre:'Tupac Amaru Digital',handle:'@tupac_digital',bio:'Blockchain para la autodeterminacion.',verificado:true,avatar_initials:'TA',seguidores:2891,siguiendo:198,voces_count:214},
  {id:3,nombre:'Anacaona Libre',handle:'@anacaona_web3',bio:'Educadora en tecnologias soberanas.',verificado:true,avatar_initials:'AL',seguidores:967,siguiendo:455,voces_count:56},
  {id:4,nombre:'Ketzalli Codewalker',handle:'@ketzalli_code',bio:'Desarrolladora full-stack.',verificado:false,avatar_initials:'KC',seguidores:543,siguiendo:321,voces_count:134},
  {id:5,nombre:'Wayna Qhapaq',handle:'@wayna_chain',bio:'Investigador en gobernanza descentralizada.',verificado:true,avatar_initials:'WQ',seguidores:1756,siguiendo:267,voces_count:98}
];
const voces = [
  {id:1,autor:'Ixchel Tlanextli',handle:'@ixchel_soberana',contenido:'La soberania digital no es un lujo, es un derecho ancestral. Nuestros datos son territorio sagrado. #SoberaniaDigital #Ierahkwa',timestamp:new Date(Date.now()-3600000).toISOString(),apoyos:47,amplificaciones:23,verificado:true,avatar_initials:'IT',hashtags:['#SoberaniaDigital','#Ierahkwa'],ciudadano_id:1},
  {id:2,autor:'Tupac Amaru Digital',handle:'@tupac_digital',contenido:'Cada bloque en nuestra cadena es un acto de resistencia. Blockchain es la nueva forma de organizacion comunitaria. #BlockchainIndigena #Wampum',timestamp:new Date(Date.now()-7200000).toISOString(),apoyos:89,amplificaciones:45,verificado:true,avatar_initials:'TA',hashtags:['#BlockchainIndigena','#Wampum'],ciudadano_id:2},
  {id:3,autor:'Anacaona Libre',handle:'@anacaona_web3',contenido:'Hoy lanzamos el primer taller de alfabetizacion digital soberana en lengua nahuatl. 43 abuelos aprendiendo wallets descentralizadas. #EducacionSoberana #Nahuatl',timestamp:new Date(Date.now()-10800000).toISOString(),apoyos:156,amplificaciones:78,verificado:true,avatar_initials:'AL',hashtags:['#EducacionSoberana','#Nahuatl'],ciudadano_id:3},
  {id:4,autor:'Ketzalli Codewalker',handle:'@ketzalli_code',contenido:'Acabo de hacer merge del PR que implementa cifrado end-to-end en el mensajero de Ierahkwa. Conversaciones privadas como susurros del viento. #OpenSource #Privacidad',timestamp:new Date(Date.now()-14400000).toISOString(),apoyos:72,amplificaciones:31,verificado:false,avatar_initials:'KC',hashtags:['#OpenSource','#Privacidad'],ciudadano_id:4},
  {id:5,autor:'Wayna Qhapaq',handle:'@wayna_chain',contenido:'Propuesta de gobernanza: turnos rotatorios inspirados en el ayni andino para validacion de nodos. La reciprocidad como protocolo de consenso. #GobernanzaDescentralizada #Ayni',timestamp:new Date(Date.now()-18000000).toISOString(),apoyos:93,amplificaciones:52,verificado:true,avatar_initials:'WQ',hashtags:['#GobernanzaDescentralizada','#Ayni'],ciudadano_id:5},
  {id:6,autor:'Ixchel Tlanextli',handle:'@ixchel_soberana',contenido:'Las plataformas corporativas extraen nuestros datos como las minas extraen nuestros minerales. Construyamos alternativas comunitarias. #DerechosDigitales #SoberaniaDigital',timestamp:new Date(Date.now()-21600000).toISOString(),apoyos:134,amplificaciones:67,verificado:true,avatar_initials:'IT',hashtags:['#DerechosDigitales','#SoberaniaDigital'],ciudadano_id:1},
  {id:7,autor:'Tupac Amaru Digital',handle:'@tupac_digital',contenido:'El token Wampum no es especulacion, es memoria. Cada transaccion registra un intercambio de valor real entre personas reales. #Wampum #EconomiaSoberana',timestamp:new Date(Date.now()-25200000).toISOString(),apoyos:201,amplificaciones:89,verificado:true,avatar_initials:'TA',hashtags:['#Wampum','#EconomiaSoberana'],ciudadano_id:2},
  {id:8,autor:'Anacaona Libre',handle:'@anacaona_web3',contenido:'Cultura no es solo lo que colgamos en museos. Cultura es el codigo que escribimos, los protocolos que disenamos. La tecnologia soberana ES cultura viva. #CulturaDigital #Ierahkwa',timestamp:new Date(Date.now()-28800000).toISOString(),apoyos:88,amplificaciones:41,verificado:true,avatar_initials:'AL',hashtags:['#CulturaDigital','#Ierahkwa'],ciudadano_id:3},
  {id:9,autor:'Ketzalli Codewalker',handle:'@ketzalli_code',contenido:'Documentacion del API de VozSoberana ya disponible en tres lenguas: espanol, quechua y maya. Si tu tecnologia no habla tu idioma, no es tu tecnologia. #Multilingue #OpenSource #SoberaniaDigital',timestamp:new Date(Date.now()-32400000).toISOString(),apoyos:167,amplificaciones:94,verificado:false,avatar_initials:'KC',hashtags:['#Multilingue','#OpenSource','#SoberaniaDigital'],ciudadano_id:4},
  {id:10,autor:'Wayna Qhapaq',handle:'@wayna_chain',contenido:'Nuevo nodo de Ierahkwa activo en la sierra de Oaxaca. 200 familias con infraestructura digital propia. Sin intermediarios, sin extractivismo. Solo comunidad. #Ierahkwa #NodosComunitarios',timestamp:new Date(Date.now()-36000000).toISOString(),apoyos:245,amplificaciones:112,verificado:true,avatar_initials:'WQ',hashtags:['#Ierahkwa','#NodosComunitarios'],ciudadano_id:5}
];
function extractHashtags(text){var m=text.match(/#[\wÀ-ɏ]+/g);return m||[];}
app.post("/api/voces",function(q,r){
  var c=q.body.contenido;
  if(\!c||\!c.trim())return r.status(400).json({error:"Contenido obligatorio"});
  var v={id:nextVozId++,autor:q.body.autor||"Anonimo",handle:q.body.handle||"@anonimo",
    contenido:c.trim(),timestamp:new Date().toISOString(),apoyos:0,amplificaciones:0,
    verificado:q.body.verificado||false,avatar_initials:q.body.avatar_initials||"CA",
    hashtags:extractHashtags(c),ciudadano_id:q.body.ciudadano_id||null};
  voces.unshift(v);r.status(201).json(v);
});
app.get("/api/voces",function(q,r){
  var page=parseInt(q.query.page)||1,limit=parseInt(q.query.limit)||20;
  var offset=(page-1)*limit;
  r.json({voces:voces.slice(offset,offset+limit),total:voces.length,page:page,limit:limit,total_pages:Math.ceil(voces.length/limit)});
});
app.get("/api/voces/:id",function(q,r){
  var v=voces.find(function(x){return x.id===parseInt(q.params.id)});
  if(\!v)return r.status(404).json({error:"Voz no encontrada"});
  r.json(v);
});
app.post("/api/voces/:id/amplificar",function(q,r){
  var v=voces.find(function(x){return x.id===parseInt(q.params.id)});
  if(\!v)return r.status(404).json({error:"Voz no encontrada"});
  v.amplificaciones+=1;r.json({id:v.id,amplificaciones:v.amplificaciones});
});
app.post("/api/voces/:id/apoyar",function(q,r){
  var v=voces.find(function(x){return x.id===parseInt(q.params.id)});
  if(\!v)return r.status(404).json({error:"Voz no encontrada"});
  v.apoyos+=1;r.json({id:v.id,apoyos:v.apoyos});
});
app.get("/api/trending",function(q,r){
  var h={};
  voces.forEach(function(v){extractHashtags(v.contenido).forEach(function(t){var n=t.toLowerCase();h[n]=(h[n]||0)+1})});
  var t=Object.entries(h).map(function(e){return{hashtag:e[0],count:e[1]}}).sort(function(a,b){return b.count-a.count}).slice(0,10);
  r.json({trending:t});
});
app.get("/api/ciudadano/:id",function(q,r){
  var id=parseInt(q.params.id);
  var c=ciudadanos.find(function(x){return x.id===id});
  if(\!c)return r.status(404).json({error:"Ciudadano no encontrado"});
  var cv=voces.filter(function(v){return v.ciudadano_id===id});
  r.json(Object.assign({},c,{voces_recientes:cv.slice(0,5)}));
});
app.listen(PORT,function(){
  console.log("");
  console.log("  VozSoberana - Ierahkwa Platform");
  console.log("  http://localhost:"+PORT);
  console.log("  API: /api/voces | Trending: /api/trending");
  console.log("  Powered by Wampum Protocol");
  console.log("");
});
