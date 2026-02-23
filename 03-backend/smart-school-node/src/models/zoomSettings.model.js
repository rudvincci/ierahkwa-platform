const { DataTypes } = require('sequelize');

module.exports = (sequelize) => {
  const ZoomSettings = sequelize.define('ZoomSettings', {
    id: {
      type: DataTypes.INTEGER,
      primaryKey: true,
      autoIncrement: true
    },
    apiKey: {
      type: DataTypes.STRING(200),
      allowNull: false
    },
    apiSecret: {
      type: DataTypes.STRING(200),
      allowNull: false
    },
    accountId: DataTypes.STRING(200),
    clientId: DataTypes.STRING(200),
    clientSecret: DataTypes.STRING(200),
    webhookSecret: DataTypes.STRING(200),
    isActive: {
      type: DataTypes.BOOLEAN,
      defaultValue: true
    },
    autoRecord: {
      type: DataTypes.BOOLEAN,
      defaultValue: false
    },
    enableWaitingRoom: {
      type: DataTypes.BOOLEAN,
      defaultValue: true
    },
    muteParticipantsOnEntry: {
      type: DataTypes.BOOLEAN,
      defaultValue: true
    },
    tenantId: {
      type: DataTypes.INTEGER,
      allowNull: false
    }
  }, {
    tableName: 'zoom_settings',
    timestamps: true
  });

  return ZoomSettings;
};
