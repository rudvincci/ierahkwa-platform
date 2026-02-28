/**
 * ═══════════════════════════════════════════════════════════════════════════════
 *  MARKETPLACE SCREEN - Sovereign Commerce Platform
 * ═══════════════════════════════════════════════════════════════════════════════
 *
 * @description Decentralized marketplace for sovereign commerce. Browse products
 *   and services from indigenous businesses, NFT marketplace, digital goods,
 *   artisan crafts, and community-powered e-commerce.
 *
 * @module screens/MarketplaceScreen
 * @requires react-native
 * @requires react-native-paper
 * @requires react-i18next
 */

import React, { useState } from 'react';
import {
  View,
  StyleSheet,
  ScrollView,
  TouchableOpacity,
  RefreshControl,
  FlatList,
} from 'react-native';
import { Text, Surface, Searchbar, Chip, Button } from 'react-native-paper';
import Icon from 'react-native-vector-icons/MaterialCommunityIcons';
import { useTranslation } from 'react-i18next';

/**
 * MarketplaceScreen component
 * @param {Object} props - Navigation props from React Navigation
 * @returns {JSX.Element} The sovereign commerce marketplace screen
 */
export default function MarketplaceScreen({ navigation }) {
  const { t } = useTranslation();
  const [refreshing, setRefreshing] = useState(false);
  const [searchQuery, setSearchQuery] = useState('');
  const [activeCategory, setActiveCategory] = useState('all');

  const onRefresh = async () => {
    setRefreshing(true);
    setTimeout(() => setRefreshing(false), 1500);
  };

  const categories = [
    { key: 'all', label: t('market_all') || 'All', icon: 'apps' },
    { key: 'artisan', label: t('market_artisan') || 'Artisan', icon: 'palette' },
    { key: 'digital', label: t('market_digital') || 'Digital', icon: 'cloud-download' },
    { key: 'nft', label: t('market_nft') || 'NFTs', icon: 'diamond-stone' },
    { key: 'services', label: t('market_services') || 'Services', icon: 'briefcase' },
    { key: 'food', label: t('market_food') || 'Food', icon: 'food-apple' },
  ];

  const [featuredProducts] = useState([
    { id: '1', name: 'Wampum Bead Necklace', seller: 'Tekahionwake Crafts', price: '250 ISB', image: null, category: 'artisan', rating: 4.9 },
    { id: '2', name: 'Sovereign Cloud Storage 1TB', seller: 'Ierahkwa Cloud', price: '50 ISB/mo', image: null, category: 'digital', rating: 4.8 },
    { id: '3', name: 'FutureWampum Genesis NFT', seller: 'Official Collection', price: '1,000 ISB', image: null, category: 'nft', rating: 5.0 },
    { id: '4', name: 'Traditional Medicine Kit', seller: 'Raices Naturales', price: '75 ISB', image: null, category: 'artisan', rating: 4.7 },
  ]);

  const [trendingProducts] = useState([
    { id: '10', name: 'Quantum-Safe VPN', price: '25 ISB/mo', sales: 3200, category: 'digital' },
    { id: '11', name: 'Handwoven Basket', price: '180 ISB', sales: 890, category: 'artisan' },
    { id: '12', name: 'AI Tutoring Session', price: '15 ISB', sales: 5600, category: 'services' },
    { id: '13', name: 'Organic Casabe Pack', price: '30 ISB', sales: 2100, category: 'food' },
    { id: '14', name: 'Governance NFT Badge', price: '500 ISB', sales: 740, category: 'nft' },
    { id: '15', name: 'Legal Consultation', price: '100 ISB', sales: 460, category: 'services' },
  ]);

  /**
   * Returns a category-appropriate color
   * @param {string} category - Product category key
   * @returns {string} Hex color code
   */
  const getCategoryColor = (category) => {
    const colors = {
      artisan: '#FFD700',
      digital: '#00FFFF',
      nft: '#E040FB',
      services: '#00FF41',
      food: '#FF9100',
      all: '#fff',
    };
    return colors[category] || '#888';
  };

  return (
    <ScrollView
      style={styles.container}
      refreshControl={<RefreshControl refreshing={refreshing} onRefresh={onRefresh} />}
      accessibilityLabel={t('market_screen_title') || 'Marketplace Screen'}
    >
      {/* Header */}
      <View style={styles.header}>
        <View>
          <Text style={styles.headerTitle}>{t('market_title') || 'Marketplace'}</Text>
          <Text style={styles.headerSubtitle}>{t('market_subtitle') || 'Sovereign Commerce'}</Text>
        </View>
        <View style={styles.headerActions}>
          <TouchableOpacity
            style={styles.headerBtn}
            onPress={() => navigation.navigate('Cart')}
            accessibilityLabel="Shopping cart"
            accessibilityRole="button"
          >
            <Icon name="cart" size={24} color="#fff" />
          </TouchableOpacity>
          <TouchableOpacity
            style={styles.headerBtn}
            onPress={() => navigation.navigate('MyStore')}
            accessibilityLabel="My store"
            accessibilityRole="button"
          >
            <Icon name="store" size={24} color="#00FF41" />
          </TouchableOpacity>
        </View>
      </View>

      {/* Search */}
      <Searchbar
        placeholder={t('market_search') || 'Search products, services, NFTs...'}
        onChangeText={setSearchQuery}
        value={searchQuery}
        style={styles.searchBar}
        inputStyle={{ color: '#fff' }}
        iconColor="#888"
        placeholderTextColor="#666"
        accessibilityLabel="Search marketplace"
      />

      {/* Categories */}
      <ScrollView horizontal showsHorizontalScrollIndicator={false} style={styles.categoriesScroll}>
        {categories.map((cat) => (
          <Chip
            key={cat.key}
            selected={activeCategory === cat.key}
            onPress={() => setActiveCategory(cat.key)}
            style={[styles.categoryChip, activeCategory === cat.key && styles.categoryChipActive]}
            textStyle={[styles.categoryText, activeCategory === cat.key && styles.categoryTextActive]}
            icon={() => <Icon name={cat.icon} size={16} color={activeCategory === cat.key ? '#000' : '#888'} />}
            accessibilityLabel={cat.label}
            accessibilityRole="tab"
            accessibilityState={{ selected: activeCategory === cat.key }}
          >
            {cat.label}
          </Chip>
        ))}
      </ScrollView>

      {/* Featured Products */}
      <View style={styles.section}>
        <View style={styles.sectionHeader}>
          <Text style={styles.sectionTitle}>{t('market_featured') || 'Featured'}</Text>
          <TouchableOpacity onPress={() => navigation.navigate('AllProducts')}>
            <Text style={styles.viewAll}>{t('view_all') || 'View All'}</Text>
          </TouchableOpacity>
        </View>
        <ScrollView horizontal showsHorizontalScrollIndicator={false}>
          {featuredProducts.map((product) => (
            <TouchableOpacity
              key={product.id}
              onPress={() => navigation.navigate('ProductDetail', { productId: product.id })}
              accessibilityLabel={`${product.name}, ${product.price}, by ${product.seller}`}
              accessibilityRole="button"
            >
              <Surface style={styles.productCard}>
                <View style={[styles.productImage, { backgroundColor: getCategoryColor(product.category) + '15' }]}>
                  <Icon name="image" size={40} color={getCategoryColor(product.category)} />
                </View>
                <View style={[styles.productCategory, { backgroundColor: getCategoryColor(product.category) + '20' }]}>
                  <Text style={[styles.productCategoryText, { color: getCategoryColor(product.category) }]}>
                    {product.category.toUpperCase()}
                  </Text>
                </View>
                <Text style={styles.productName} numberOfLines={2}>{product.name}</Text>
                <Text style={styles.productSeller}>{product.seller}</Text>
                <View style={styles.productFooter}>
                  <Text style={styles.productPrice}>{product.price}</Text>
                  <View style={styles.ratingRow}>
                    <Icon name="star" size={14} color="#FFD700" />
                    <Text style={styles.ratingText}>{product.rating}</Text>
                  </View>
                </View>
              </Surface>
            </TouchableOpacity>
          ))}
        </ScrollView>
      </View>

      {/* Trending */}
      <View style={styles.section}>
        <Text style={styles.sectionTitle}>{t('market_trending') || 'Trending'}</Text>
        {trendingProducts.map((product, index) => (
          <TouchableOpacity
            key={product.id}
            onPress={() => navigation.navigate('ProductDetail', { productId: product.id })}
            accessibilityLabel={`${index + 1}. ${product.name}, ${product.price}`}
            accessibilityRole="button"
          >
            <Surface style={styles.trendingCard}>
              <Text style={styles.trendingRank}>#{index + 1}</Text>
              <View style={[styles.trendingIcon, { backgroundColor: getCategoryColor(product.category) + '20' }]}>
                <Icon name="image" size={24} color={getCategoryColor(product.category)} />
              </View>
              <View style={styles.trendingInfo}>
                <Text style={styles.trendingName}>{product.name}</Text>
                <Text style={styles.trendingSales}>{product.sales.toLocaleString()} {t('market_sold') || 'sold'}</Text>
              </View>
              <Text style={styles.trendingPrice}>{product.price}</Text>
            </Surface>
          </TouchableOpacity>
        ))}
      </View>

      {/* Sell CTA */}
      <Surface style={styles.sellCard}>
        <Icon name="storefront" size={32} color="#FFD700" />
        <View style={styles.sellInfo}>
          <Text style={styles.sellTitle}>{t('market_start_selling') || 'Start Selling'}</Text>
          <Text style={styles.sellDescription}>
            {t('market_sell_desc') || 'Open your sovereign store and reach 72M citizens'}
          </Text>
        </View>
        <Button
          mode="contained"
          onPress={() => navigation.navigate('CreateStore')}
          buttonColor="#FFD700"
          textColor="#000"
          compact
          accessibilityLabel="Open your store"
        >
          {t('market_open_store') || 'Open Store'}
        </Button>
      </Surface>

      <View style={{ height: 30 }} />
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  container: { flex: 1, backgroundColor: '#0A0A0F', padding: 16 },
  header: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', marginBottom: 20 },
  headerTitle: { color: '#fff', fontSize: 28, fontWeight: 'bold' },
  headerSubtitle: { color: '#888', fontSize: 14, marginTop: 4 },
  headerActions: { flexDirection: 'row' },
  headerBtn: { marginLeft: 16 },
  searchBar: { backgroundColor: '#1A1A2E', marginBottom: 16, borderRadius: 12 },
  categoriesScroll: { marginBottom: 20 },
  categoryChip: { marginRight: 8, backgroundColor: '#1A1A2E' },
  categoryChipActive: { backgroundColor: '#00FF41' },
  categoryText: { color: '#888' },
  categoryTextActive: { color: '#000', fontWeight: 'bold' },
  section: { marginBottom: 25 },
  sectionHeader: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', marginBottom: 15 },
  sectionTitle: { color: '#fff', fontSize: 18, fontWeight: 'bold', marginBottom: 15 },
  viewAll: { color: '#00FF41', fontSize: 14 },
  productCard: { backgroundColor: '#1A1A2E', borderRadius: 12, padding: 12, marginRight: 12, width: 180 },
  productImage: { height: 120, borderRadius: 8, justifyContent: 'center', alignItems: 'center', marginBottom: 10 },
  productCategory: { paddingHorizontal: 8, paddingVertical: 3, borderRadius: 6, alignSelf: 'flex-start', marginBottom: 6 },
  productCategoryText: { fontSize: 10, fontWeight: 'bold' },
  productName: { color: '#fff', fontSize: 14, fontWeight: '600', marginBottom: 4 },
  productSeller: { color: '#888', fontSize: 12, marginBottom: 8 },
  productFooter: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center' },
  productPrice: { color: '#00FF41', fontSize: 16, fontWeight: 'bold' },
  ratingRow: { flexDirection: 'row', alignItems: 'center' },
  ratingText: { color: '#FFD700', fontSize: 12, marginLeft: 4 },
  trendingCard: { backgroundColor: '#1A1A2E', borderRadius: 12, padding: 14, flexDirection: 'row', alignItems: 'center', marginBottom: 8 },
  trendingRank: { color: '#FFD700', fontSize: 18, fontWeight: 'bold', width: 30 },
  trendingIcon: { width: 44, height: 44, borderRadius: 12, justifyContent: 'center', alignItems: 'center' },
  trendingInfo: { flex: 1, marginLeft: 12 },
  trendingName: { color: '#fff', fontSize: 14, fontWeight: '500' },
  trendingSales: { color: '#888', fontSize: 12, marginTop: 2 },
  trendingPrice: { color: '#00FF41', fontSize: 14, fontWeight: 'bold' },
  sellCard: { backgroundColor: '#1A1A2E', borderRadius: 16, padding: 20, flexDirection: 'row', alignItems: 'center', borderWidth: 1, borderColor: '#FFD70040' },
  sellInfo: { flex: 1, marginHorizontal: 12 },
  sellTitle: { color: '#FFD700', fontSize: 16, fontWeight: 'bold' },
  sellDescription: { color: '#888', fontSize: 12, marginTop: 4 },
});
