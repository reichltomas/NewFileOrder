/*
 * Copyright (c) 2023 Tomáš Reichl.
 *
 * Licensed under BSD 3-Clause license. See LICENSE.
 */

import 'package:new_file_order/data/repositories/app_preferences_repository.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:test/test.dart';

void main() {
  testAppPreferencesRepositorySharedPreferences();
}

void testAppPreferencesRepositorySharedPreferences() {
  const keyDbList = "databases"; // string list
  const keyOpenDb = "open_database"; // int

  const testDb = <String>["data/test1", "data/test2", "data/test3"];

  test('Empty preferences return no open database', () async {
    SharedPreferences.setMockInitialValues(<String, Object>{});
    final prefs = await SharedPreferences.getInstance();
    final repo = AppPreferencesRepositorySharedPreferences(prefs);

    expect(repo.getOpenDatabase(), equals(null));
  });

  test('Empty preferences return empty database list', () async {
    SharedPreferences.setMockInitialValues(<String, Object>{});
    final prefs = await SharedPreferences.getInstance();
    final repo = AppPreferencesRepositorySharedPreferences(prefs);

    expect(repo.getDatabases(), equals(<String>[]));
  });

  test('Databases can be added', () async {
    SharedPreferences.setMockInitialValues(<String, Object>{});
    final prefs = await SharedPreferences.getInstance();
    final repo = AppPreferencesRepositorySharedPreferences(prefs);

    await repo.addDatabase(testDb[0]);
    await repo.addDatabase(testDb[1]);
    await repo.addDatabase(testDb[2]);

    expect(repo.getDatabases(), equals(testDb));
  });

  test('Databases can be deleted', () async {
    SharedPreferences.setMockInitialValues(<String, Object>{
      keyDbList: testDb,
    });
    final prefs = await SharedPreferences.getInstance();
    final repo = AppPreferencesRepositorySharedPreferences(prefs);

    await repo.deleteDatabase(1);
    await repo.deleteDatabase(1);

    expect(repo.getDatabases(), equals(<String>["data/test1"]));
  });

  test('Database can be set as currently open when none was set', () async {
    SharedPreferences.setMockInitialValues(<String, Object>{
      keyDbList: testDb,
    });
    final prefs = await SharedPreferences.getInstance();
    final repo = AppPreferencesRepositorySharedPreferences(prefs);

    await repo.openDatabase(2);

    expect(repo.getOpenDatabase(), equals(testDb[2]));
  });

  test('Database can be set as currently open when one was already set',
      () async {
    SharedPreferences.setMockInitialValues(<String, Object>{
      keyDbList: testDb,
      keyOpenDb: 0,
    });
    final prefs = await SharedPreferences.getInstance();
    final repo = AppPreferencesRepositorySharedPreferences(prefs);

    await repo.openDatabase(1);

    expect(repo.getOpenDatabase(), equals(testDb[1]));
  });

  test('Database open fails on negative index value', () async {
    SharedPreferences.setMockInitialValues(<String, Object>{
      keyDbList: testDb,
    });
    final prefs = await SharedPreferences.getInstance();
    final repo = AppPreferencesRepositorySharedPreferences(prefs);

    var result = await repo.openDatabase(-3);

    expect(result, equals(false));
  });

  test('Database open fails on value >= database count', () async {
    SharedPreferences.setMockInitialValues(<String, Object>{
      keyDbList: testDb,
    });
    final prefs = await SharedPreferences.getInstance();
    final repo = AppPreferencesRepositorySharedPreferences(prefs);

    var result = await repo.openDatabase(3);

    expect(result, equals(false));
  });

  test('Database delete fails on negative index value', () async {
    SharedPreferences.setMockInitialValues(<String, Object>{
      keyDbList: testDb,
    });
    final prefs = await SharedPreferences.getInstance();
    final repo = AppPreferencesRepositorySharedPreferences(prefs);

    var result = await repo.deleteDatabase(-3);

    expect(result, equals(false));
  });

  test('Database delete fails on value >= database count', () async {
    SharedPreferences.setMockInitialValues(<String, Object>{
      keyDbList: testDb,
    });
    final prefs = await SharedPreferences.getInstance();
    final repo = AppPreferencesRepositorySharedPreferences(prefs);

    var result = await repo.deleteDatabase(3);

    expect(result, equals(false));
  });

  test('Databases are by default not opened when adding', () async {
    SharedPreferences.setMockInitialValues(<String, Object>{
      keyDbList: testDb,
      keyOpenDb: 0,
    });
    final prefs = await SharedPreferences.getInstance();
    final repo = AppPreferencesRepositorySharedPreferences(prefs);

    const addedDb = "test/another_db";

    await repo.addDatabase(addedDb);

    expect(repo.getOpenDatabase(), equals(testDb[0]));
  });

  test('Database can be opened when adding', () async {
    SharedPreferences.setMockInitialValues(<String, Object>{
      keyDbList: testDb,
      keyOpenDb: 0,
    });
    final prefs = await SharedPreferences.getInstance();
    final repo = AppPreferencesRepositorySharedPreferences(prefs);

    const addedDb = "test/another_db";

    await repo.addDatabase(addedDb, true);

    expect(repo.getOpenDatabase(), equals(addedDb));
  });

  test('Database can be closed', () async {
    SharedPreferences.setMockInitialValues(<String, Object>{
      keyDbList: testDb,
      keyOpenDb: 0,
    });
    final prefs = await SharedPreferences.getInstance();
    final repo = AppPreferencesRepositorySharedPreferences(prefs);

    await repo.closeDatabase();

    expect(repo.getOpenDatabase(), equals(null));
  });

  test('Currently open database is closed when removed', () async {
    SharedPreferences.setMockInitialValues(<String, Object>{
      keyDbList: testDb,
      keyOpenDb: 0,
    });
    final prefs = await SharedPreferences.getInstance();
    final repo = AppPreferencesRepositorySharedPreferences(prefs);

    await repo.deleteDatabase(0);

    expect(repo.getOpenDatabase(), equals(null));
  });

  test(
      'When deleting, currently open database is preserved, if it is behind '
      'the deleted database in list', () async {
    SharedPreferences.setMockInitialValues(<String, Object>{
      keyDbList: testDb,
      keyOpenDb: 2,
    });
    final prefs = await SharedPreferences.getInstance();
    final repo = AppPreferencesRepositorySharedPreferences(prefs);

    await repo.deleteDatabase(1);

    expect([repo.getOpenDatabase(), repo.getDatabases().length],
        equals([testDb[2], 2]));
  });
}
