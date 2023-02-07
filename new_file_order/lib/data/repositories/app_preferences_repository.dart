/*
 * Copyright (c) 2023 Tomáš Reichl.
 *
 * Licensed under BSD 3-Clause license. See LICENSE.
 */

import 'package:shared_preferences/shared_preferences.dart';

part 'app_preferences_repository_shared_preferences.dart';

/// Repository providing application preferences, that are not database specific
///
/// These include UI and locale preferences and also list of databases and
/// currently opened database
abstract class AppPreferencesRepository {
  /// Get list of all registered databases
  List<String> getDatabases();

  /// Get path to currently open database
  ///
  /// Return path or null when no database is open
  String? getOpenDatabase();

  /// Set currently open database
  ///
  /// [index] is from list obtained with [getDatabases]. Returns false if index
  /// is invalid.
  Future<bool> openDatabase(int index);

  /// Set currently open database to null
  Future<void> closeDatabase();

  /// Adds new database entry into preferences
  ///
  /// [path] is path to database. When [setCurrent] is true, the new database
  /// will also be set as currently open.
  ///
  /// Returns false when database could not be added, for example when there is
  /// already a duplicate.
  Future<bool> addDatabase(String path, [bool setCurrent = false]);

  /// Deletes database entry from preferences
  ///
  /// [index] is from list obtained with [getDatabases]. If the database is
  /// set as open, currently open database is set to null.
  ///
  /// Returns false if index is invalid.
  Future<bool> deleteDatabase(int index);
}
