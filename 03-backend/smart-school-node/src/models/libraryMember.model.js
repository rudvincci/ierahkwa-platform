const { DataTypes } = require('sequelize');

module.exports = (sequelize) => {
  const LibraryMember = sequelize.define('LibraryMember', {
    id: {
      type: DataTypes.INTEGER,
      primaryKey: true,
      autoIncrement: true
    },
    memberNumber: {
      type: DataTypes.STRING(50),
      allowNull: false
    },
    type: {
      type: DataTypes.ENUM('Student', 'Teacher', 'Staff', 'External'),
      allowNull: false
    },
    studentId: DataTypes.INTEGER,
    teacherId: DataTypes.INTEGER,
    staffId: DataTypes.INTEGER,
    name: {
      type: DataTypes.STRING(200),
      allowNull: false
    },
    email: DataTypes.STRING(200),
    phone: DataTypes.STRING(50),
    joinDate: {
      type: DataTypes.DATEONLY,
      defaultValue: DataTypes.NOW
    },
    expiryDate: DataTypes.DATEONLY,
    maxBooksAllowed: {
      type: DataTypes.INTEGER,
      defaultValue: 3
    },
    currentBorrowCount: {
      type: DataTypes.INTEGER,
      defaultValue: 0
    },
    isActive: {
      type: DataTypes.BOOLEAN,
      defaultValue: true
    },
    tenantId: {
      type: DataTypes.INTEGER,
      allowNull: false
    }
  }, {
    tableName: 'library_members',
    timestamps: true,
    paranoid: true
  });

  return LibraryMember;
};
