'use strict';

const { Router } = require('express');

const CATEGORIES = [
  // BELLEZA Y CUIDADO PERSONAL
  { id: 'barber', name: 'Barbería', nameEn: 'Barber', emoji: '💈', atHome: true, inShop: true },
  { id: 'hair-stylist', name: 'Estilista', nameEn: 'Hair Stylist', emoji: '💇', atHome: true, inShop: true },
  { id: 'tattoo', name: 'Tatuajes', nameEn: 'Tattoo Artist', emoji: '🎨', atHome: false, inShop: true },
  { id: 'nails', name: 'Uñas', nameEn: 'Nail Tech', emoji: '💅', atHome: true, inShop: true },
  { id: 'makeup', name: 'Maquillaje', nameEn: 'Makeup Artist', emoji: '💄', atHome: true, inShop: true },
  { id: 'massage', name: 'Masajes', nameEn: 'Massage Therapist', emoji: '💆', atHome: true, inShop: true },
  { id: 'spa', name: 'Spa', nameEn: 'Spa Services', emoji: '🧖', atHome: true, inShop: true },
  // HOGAR Y REPARACIONES
  { id: 'plumber', name: 'Plomero', nameEn: 'Plumber', emoji: '🔧', atHome: true, inShop: false },
  { id: 'electrician', name: 'Electricista', nameEn: 'Electrician', emoji: '⚡', atHome: true, inShop: false },
  { id: 'carpenter', name: 'Carpintero', nameEn: 'Carpenter', emoji: '🪚', atHome: true, inShop: true },
  { id: 'painter', name: 'Pintor', nameEn: 'Painter', emoji: '🎨', atHome: true, inShop: false },
  { id: 'locksmith', name: 'Cerrajero', nameEn: 'Locksmith', emoji: '🔑', atHome: true, inShop: true },
  { id: 'cleaning', name: 'Limpieza', nameEn: 'Cleaning Service', emoji: '🧹', atHome: true, inShop: false },
  { id: 'ac-repair', name: 'Aire Acondicionado', nameEn: 'AC Repair', emoji: '❄️', atHome: true, inShop: false },
  // AUTOMOTRIZ
  { id: 'mechanic', name: 'Mecánico', nameEn: 'Mechanic', emoji: '🔧', atHome: true, inShop: true },
  { id: 'car-wash', name: 'Lavado de Autos', nameEn: 'Car Wash', emoji: '🚗', atHome: true, inShop: true },
  // EDUCACIÓN
  { id: 'tutor', name: 'Tutor', nameEn: 'Tutor', emoji: '📚', atHome: true, inShop: true },
  { id: 'language-teacher', name: 'Profesor de Idiomas', nameEn: 'Language Teacher', emoji: '🌐', atHome: true, inShop: true },
  // COMIDA
  { id: 'private-chef', name: 'Chef Privado', nameEn: 'Private Chef', emoji: '👨‍🍳', atHome: true, inShop: false },
  { id: 'catering', name: 'Catering', nameEn: 'Catering', emoji: '🍽️', atHome: true, inShop: false },
  // SALUD
  { id: 'personal-trainer', name: 'Entrenador Personal', nameEn: 'Personal Trainer', emoji: '🏋️', atHome: true, inShop: true },
  { id: 'traditional-healer', name: 'Curandero', nameEn: 'Traditional Healer', emoji: '🌿', atHome: true, inShop: true },
  { id: 'midwife', name: 'Partera', nameEn: 'Midwife', emoji: '👶', atHome: true, inShop: false },
  // EVENTOS
  { id: 'photographer', name: 'Fotógrafo', nameEn: 'Photographer', emoji: '📸', atHome: true, inShop: true },
  { id: 'musician', name: 'Músico', nameEn: 'Musician', emoji: '🎵', atHome: true, inShop: false },
  { id: 'dj', name: 'DJ', nameEn: 'DJ', emoji: '🎧', atHome: true, inShop: false },
  // DELIVERY
  { id: 'delivery', name: 'Delivery / Mandados', nameEn: 'Delivery', emoji: '📦', atHome: true, inShop: false },
  { id: 'moving', name: 'Mudanzas', nameEn: 'Moving Service', emoji: '🚚', atHome: true, inShop: false },
  // TECNOLOGÍA
  { id: 'tech-support', name: 'Soporte Técnico', nameEn: 'Tech Support', emoji: '💻', atHome: true, inShop: true },
  // MASCOTAS
  { id: 'vet', name: 'Veterinario', nameEn: 'Veterinarian', emoji: '🐾', atHome: true, inShop: true },
  { id: 'dog-groomer', name: 'Peluquería Canina', nameEn: 'Pet Groomer', emoji: '🐕', atHome: true, inShop: true },
];

module.exports = function () {
  const router = Router();

  router.get('/', (req, res) => {
    res.json({ categories: CATEGORIES, total: CATEGORIES.length, providerPercent: '92%', taxRate: '0%' });
  });

  router.get('/:id', (req, res) => {
    const cat = CATEGORIES.find(c => c.id === req.params.id);
    if (!cat) return res.status(404).json({ error: 'Category not found' });
    res.json(cat);
  });

  return router;
};

module.exports.CATEGORIES = CATEGORIES;
