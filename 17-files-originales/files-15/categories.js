const router = require('express').Router();

const CATEGORIES = [
  // BELLEZA Y CUIDADO PERSONAL
  { id: 'barber', name: 'Barbería', nameEn: 'Barber', emoji: '💈', description: 'Cortes, afeitado, barba', atHome: true, inShop: true },
  { id: 'hair-stylist', name: 'Estilista', nameEn: 'Hair Stylist', emoji: '💇', description: 'Cortes, color, peinados', atHome: true, inShop: true },
  { id: 'tattoo', name: 'Tatuajes', nameEn: 'Tattoo Artist', emoji: '🎨', description: 'Tatuajes, diseño, cobertura', atHome: false, inShop: true },
  { id: 'piercing', name: 'Piercing', nameEn: 'Piercing', emoji: '💎', description: 'Perforaciones profesionales', atHome: false, inShop: true },
  { id: 'nails', name: 'Uñas', nameEn: 'Nail Tech', emoji: '💅', description: 'Manicure, pedicure, gel, acrílico', atHome: true, inShop: true },
  { id: 'makeup', name: 'Maquillaje', nameEn: 'Makeup Artist', emoji: '💄', description: 'Maquillaje profesional, bodas, eventos', atHome: true, inShop: true },
  { id: 'massage', name: 'Masajes', nameEn: 'Massage Therapist', emoji: '💆', description: 'Relajante, deportivo, terapéutico, ancestral', atHome: true, inShop: true },
  { id: 'spa', name: 'Spa', nameEn: 'Spa Services', emoji: '🧖', description: 'Faciales, tratamientos corporales', atHome: true, inShop: true },
  { id: 'waxing', name: 'Depilación', nameEn: 'Waxing', emoji: '🦵', description: 'Cera, láser, sugaring', atHome: true, inShop: true },

  // HOGAR Y REPARACIONES
  { id: 'plumber', name: 'Plomero', nameEn: 'Plumber', emoji: '🔧', description: 'Reparaciones, instalación, emergencias', atHome: true, inShop: false },
  { id: 'electrician', name: 'Electricista', nameEn: 'Electrician', emoji: '⚡', description: 'Instalación, reparación, emergencias', atHome: true, inShop: false },
  { id: 'carpenter', name: 'Carpintero', nameEn: 'Carpenter', emoji: '🪚', description: 'Muebles, reparaciones, instalación', atHome: true, inShop: true },
  { id: 'painter', name: 'Pintor', nameEn: 'Painter', emoji: '🎨', description: 'Interior, exterior, decorativo', atHome: true, inShop: false },
  { id: 'locksmith', name: 'Cerrajero', nameEn: 'Locksmith', emoji: '🔑', description: 'Cerraduras, llaves, emergencias 24/7', atHome: true, inShop: true },
  { id: 'cleaning', name: 'Limpieza', nameEn: 'Cleaning Service', emoji: '🧹', description: 'Hogar, oficina, profunda', atHome: true, inShop: false },
  { id: 'ac-repair', name: 'Aire Acondicionado', nameEn: 'AC Repair', emoji: '❄️', description: 'Instalación, reparación, mantenimiento', atHome: true, inShop: false },
  { id: 'appliance-repair', name: 'Electrodomésticos', nameEn: 'Appliance Repair', emoji: '🔌', description: 'Lavadoras, refrigeradores, estufas', atHome: true, inShop: false },

  // AUTOMOTRIZ
  { id: 'mechanic', name: 'Mecánico', nameEn: 'Mechanic', emoji: '🔧', description: 'Reparación, mantenimiento, diagnóstico', atHome: true, inShop: true },
  { id: 'car-wash', name: 'Lavado de Autos', nameEn: 'Car Wash', emoji: '🚗', description: 'Lavado, encerado, detallado', atHome: true, inShop: true },
  { id: 'tow-truck', name: 'Grúa', nameEn: 'Tow Truck', emoji: '🚛', description: 'Remolque, asistencia vial', atHome: true, inShop: false },

  // EDUCACIÓN Y PROFESIONAL
  { id: 'tutor', name: 'Tutor', nameEn: 'Tutor', emoji: '📚', description: 'Matemáticas, ciencias, idiomas, música', atHome: true, inShop: true },
  { id: 'language-teacher', name: 'Profesor de Idiomas', nameEn: 'Language Teacher', emoji: '🌐', description: 'Idiomas indígenas y globales', atHome: true, inShop: true },
  { id: 'accountant', name: 'Contador', nameEn: 'Accountant', emoji: '📊', description: 'Contabilidad, impuestos (0% aquí!), auditoría', atHome: false, inShop: true },
  { id: 'lawyer', name: 'Abogado', nameEn: 'Lawyer', emoji: '⚖️', description: 'Consultas legales, documentos', atHome: false, inShop: true },

  // COMIDA
  { id: 'private-chef', name: 'Chef Privado', nameEn: 'Private Chef', emoji: '👨‍🍳', description: 'Cocina a domicilio, eventos, clases', atHome: true, inShop: false },
  { id: 'catering', name: 'Catering', nameEn: 'Catering', emoji: '🍽️', description: 'Eventos, bodas, fiestas', atHome: true, inShop: false },

  // SALUD Y BIENESTAR
  { id: 'personal-trainer', name: 'Entrenador Personal', nameEn: 'Personal Trainer', emoji: '🏋️', description: 'Fitness, yoga, artes marciales', atHome: true, inShop: true },
  { id: 'traditional-healer', name: 'Curandero', nameEn: 'Traditional Healer', emoji: '🌿', description: 'Medicina ancestral, plantas, ceremonias', atHome: true, inShop: true },
  { id: 'midwife', name: 'Partera', nameEn: 'Midwife', emoji: '👶', description: 'Partería tradicional y moderna', atHome: true, inShop: false },

  // EVENTOS Y CREATIVOS
  { id: 'photographer', name: 'Fotógrafo', nameEn: 'Photographer', emoji: '📸', description: 'Bodas, retratos, productos, eventos', atHome: true, inShop: true },
  { id: 'musician', name: 'Músico', nameEn: 'Musician', emoji: '🎵', description: 'Eventos, serenatas, clases', atHome: true, inShop: false },
  { id: 'dj', name: 'DJ', nameEn: 'DJ', emoji: '🎧', description: 'Fiestas, bodas, eventos', atHome: true, inShop: false },
  { id: 'event-planner', name: 'Organizador de Eventos', nameEn: 'Event Planner', emoji: '🎉', description: 'Bodas, quinceañeras, ceremonias', atHome: false, inShop: true },

  // DELIVERY Y TRANSPORTE
  { id: 'delivery', name: 'Delivery / Mandados', nameEn: 'Delivery / Errands', emoji: '📦', description: 'Envíos, mandados, compras', atHome: true, inShop: false },
  { id: 'moving', name: 'Mudanzas', nameEn: 'Moving Service', emoji: '🚚', description: 'Mudanza, carga, descarga', atHome: true, inShop: false },

  // TECNOLOGÍA
  { id: 'tech-support', name: 'Soporte Técnico', nameEn: 'Tech Support', emoji: '💻', description: 'Computadoras, celulares, redes', atHome: true, inShop: true },
  { id: 'web-dev', name: 'Desarrollador Web', nameEn: 'Web Developer', emoji: '🌐', description: 'Sitios web, apps, soporte', atHome: false, inShop: false },

  // MASCOTAS
  { id: 'vet', name: 'Veterinario', nameEn: 'Veterinarian', emoji: '🐾', description: 'Consultas, vacunas, emergencias', atHome: true, inShop: true },
  { id: 'dog-groomer', name: 'Peluquería Canina', nameEn: 'Pet Groomer', emoji: '🐕', description: 'Baño, corte, cuidado', atHome: true, inShop: true },
  { id: 'pet-sitter', name: 'Cuidador de Mascotas', nameEn: 'Pet Sitter', emoji: '🏠', description: 'Hospedaje, paseo, cuidado', atHome: true, inShop: false },
];

router.get('/', (req, res) => {
  const { language = 'es' } = req.query;
  res.json({ categories: CATEGORIES, total: CATEGORIES.length, providerPercent: '92%', taxRate: '0%' });
});

router.get('/:id', (req, res) => {
  const cat = CATEGORIES.find(c => c.id === req.params.id);
  res.json(cat || { error: 'Category not found' });
});

module.exports = router;
module.exports.CATEGORIES = CATEGORIES;
