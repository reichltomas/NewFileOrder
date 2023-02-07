/*
 * Copyright (c) 2023 Tomáš Reichl.
 *
 * Licensed under BSD 3-Clause license. See LICENSE.
 */

part of 'app_preferences_repository.dart';

/// Implementation of [AppPreferencesRepository] using [SharedPreferences]
class AppPreferencesRepositorySharedPreferences
    implements AppPreferencesRepository {

  static const _keyDbList = "databases"; // string list
  static const _keyOpenDb = "open_database"; // int

  final SharedPreferences _prefs;
  // TODO consider using success/error info from SharedPreferences set methods

  AppPreferencesRepositorySharedPreferences(this._prefs);

  @override
  List<String> getDatabases() {
    final databases = _prefs.getStringList(_keyDbList);
    return (databases == null) ? <String>[] : databases;
  }

  @override
  String? getOpenDatabase() {
    final databases = getDatabases();
    final currentDb = _prefs.getInt(_keyOpenDb);

    if (currentDb == null) {
      closeDatabase();
      return null;
    }

    if (currentDb < 0) {
      return null;
    }

    if (currentDb >= databases.length) {
      closeDatabase();
      return null;
    }

    return databases[currentDb];
  }

  @override
  Future<void> closeDatabase() async {
    _prefs.setInt(_keyOpenDb, -1);
  }

  @override
  Future<bool> openDatabase(int index) async {
    final databases = getDatabases();

    if (index < 0 || index >= databases.length) {
      return false;
    }

    await _prefs.setInt(_keyOpenDb, index);
    return true;
  }

  @override
  Future<bool> addDatabase(String path, [bool setCurrent = false]) async {
    final databases = getDatabases();

    if (databases.contains(path)) {
      return false;
    }
    
    databases.add(path);

    await _prefs.setStringList(_keyDbList, databases);

    if (setCurrent) {
      await openDatabase(databases.length - 1);
    }

    return true;
  }

  @override
  Future<bool> deleteDatabase(int index) async {
    final databases = getDatabases();
    final currentDb = _prefs.getInt(_keyOpenDb);

    if (index < 0 || index >= databases.length) {
      return false;
    }

    if (currentDb == index) {
      await closeDatabase();
    } else if (currentDb != null && currentDb > index) {
      // index of every database entry after the deleted one will be decremented
      await openDatabase(currentDb - 1);
    }

    databases.removeAt(index);
    await _prefs.setStringList(_keyDbList, databases);
    return true;
  }
}
