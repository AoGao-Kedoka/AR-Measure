import 'package:flutter/material.dart';
import 'package:flutter/foundation.dart';

class MenuScreen extends StatefulWidget {
  const MenuScreen({Key? key}) : super(key: key);

  @override
  _MenuScreenState createState() => _MenuScreenState();
}

class _MenuScreenState extends State<MenuScreen> {
  final String title = 'Start Measuring';
  final String route = '/unity';

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('AR-Measure'),
      ),
      body: Center(
        child: Column(
          children: [
            const SizedBox(
              height: 32,
            ),
            const Padding(
              padding: EdgeInsets.all(30),
              child: Text(
                'Welcome!',
                style: TextStyle(
                  fontSize: 20,
                  letterSpacing: 2,
                  wordSpacing: 2,
                  fontStyle: FontStyle.italic,
                ),
              ),
            ),
            const SizedBox(
              height: 96,
            ),
            Padding(
              padding: const EdgeInsets.all(30),
              child: ElevatedButton(
                onPressed: () {
                  Navigator.of(context).pushNamed(route);
                },
                style: ElevatedButton.styleFrom(
                  elevation: 10,
                  primary: const Color(0xFFf7b900),
                  minimumSize: const Size(256, 64),
                ),
                child: Text(
                  title,
                  style: const TextStyle(
                    fontSize: 20,
                    fontWeight: FontWeight.bold,
                  ),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
