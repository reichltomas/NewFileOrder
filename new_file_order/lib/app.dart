/*
 * Copyright (c) 2023 Tomáš Reichl.
 *
 * Licensed under BSD 3-Clause license. See LICENSE.
 */

import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:new_file_order/data/repositories/app_preferences_repository.dart';

class App extends StatefulWidget {
  const App({super.key});

  @override
  State<StatefulWidget> createState() => AppState();

}

class AppState extends State<App> {

  @override
  void initState() {
    super.initState();

    var db = context.read<AppPreferencesRepository>().getOpenDatabase();


  }

  @override
  Widget build(BuildContext context) {
    // TODO: implement build
    throw UnimplementedError();
  }

}
