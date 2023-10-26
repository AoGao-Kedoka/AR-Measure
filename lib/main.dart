import 'package:flutter/material.dart';
import 'package:flutter/foundation.dart';

import 'menuscreen.dart';
import 'measure.dart';

void main() {
  runApp(const MyApp());
}

class MyApp extends StatelessWidget {
  const MyApp({Key? key}) : super(key: key);

  // This widget is the root of your application.
  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'ARMeasure',
      theme: ThemeData.dark(),
      initialRoute: '/',
      routes: {
        '/': (context) => const MenuScreen(
              key: null,
            ),
        '/unity': (context) => const UnityScreen(
              key: null,
            ),
      },
    );
  }
}
