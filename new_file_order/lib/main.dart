/*
 * Copyright (c) 2023 Tomáš Reichl.
 *
 * Licensed under BSD 3-Clause license. See LICENSE.
 */

import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:new_file_order/app.dart';
import 'package:new_file_order/data/repositories/app_preferences_repository.dart';
import 'package:shared_preferences/shared_preferences.dart';

Future<void> main() async {
  SharedPreferences prefs = await SharedPreferences.getInstance();

  runApp(
    RepositoryProvider(
      create: (context) => AppPreferencesRepositorySharedPreferences(prefs),
      child: const App(),
    ),
  );
}
